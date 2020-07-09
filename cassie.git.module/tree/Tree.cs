using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cassie.git.module.commits;
using cassie.git.module.repo;

namespace cassie.git.module.tree
{
    public class Tree
    {
        public SHA1 TreeID { get; set; }
        public Tree Parent { get; set; }
        public Repository Repo { get; set; }

        public static int DefaultProcessCount = Environment.ProcessorCount;

        public List<TreeEntry> Entries { get; set; }

        public Tree()
        {
            this.Entries = new List<TreeEntry>();
        }

        // TreeEntry returns the TreeEntry by given subpath of the tree.
        public async Task<TreeEntry> NewTreeEntry(string subPath, params LsTreeOptions[] opts)
        {
            if (string.IsNullOrEmpty(subPath)) return new TreeEntry
            {
                ID = this.TreeID,
                Typ = ObjectType.ObjectTree,
                Mode = EntryMode.EntryTree
            };
            subPath = Utils.CleanFileName(subPath);
            var paths = subPath.Split('/');
            var tree = this;
            for (int i = 0; i < paths.Length; i++)
            {
                var name = paths[i];
                if (i == paths.Length - 1)
                {
                    var entries = await tree.EntriyList(opts);
                    foreach (var item in entries)
                    {
                        if (item.Name == name) return item;
                    }
                }
                else
                {
                    tree = await tree.Subtree(name, opts);
                }
            }
            return null;
        }

        // Blob returns the blob object by given subpath of the tree.
        public async Task<Blob> NewBlob(string subPath, params LsTreeOptions[] opts)
        {
            var e = await this.NewTreeEntry(subPath, opts);
            if (e != null && (e.IsBlob() || e.IsExec()))
            {
                return e.NewBlob();
            }
            return null;
        }

        // BlobByIndex returns blob object by given index.
        public async Task<Blob> BlobByIndex(string index)
        {
            var typ = await this.Repo.CatFileType(index);
            if (typ != ObjectType.ObjectBlob) throw new Exception("Typ can not be null");
            var id = await this.Repo.RevParse(index);
            return new Blob
            {
                Mode = EntryMode.EntryBlob,
                Typ = ObjectType.ObjectBlob,
                ID = SHA1.MustIDFromString(index),
                Parent = this
            };
        }


        public async Task<Tree> Subtree(string subPath, params LsTreeOptions[] opts)
        {
            if (string.IsNullOrEmpty(subPath)) return this;
            var paths = subPath.Split('/');
            var e = new TreeEntry();
            Tree g = new Tree();
            var p = this;
            foreach (var item in paths)
            {
                e = await p.NewTreeEntry(subPath, opts);
                g = new Tree
                {
                    TreeID = e.ID,
                    Parent = p,
                    Repo = this.Repo
                };
                p = g;
            }
            return g;
        }

        // Entries returns all entries of the tree.
        public async Task<List<TreeEntry>> EntriyList(params LsTreeOptions[] opts)
        {
            if (this.Entries != null && this.Entries.Count > 0) return this.Entries;
            Tree tt = new Tree();
            var result = await this.Repo.LsTree(this.TreeID.String(), opts);
            this.Entries = result.Entries.OrderBy(a => a.Name).OrderBy(a => a.Mode).ToList();
            return this.Entries;
        }

        // CommitsInfo returns a list of commit information for these tree entries in the state of
        // given commit and subpath. It takes advantages of concurrency to speed up the process.
        // The returned list has the same number of items as tree entries, so the caller can access
        // them via slice indices.
        public async Task<List<EntryCommitInfo>> CommitsInfo(Commit commit, params CommitsInfoOptions[] opts)
        {
            if (Entries.Count == 0) return new List<EntryCommitInfo>();
            var opt = new CommitsInfoOptions();
            if (opts.Length > 0) opt = opts[0];
            if (opt.MaxConcurrency <= 0) opt.MaxConcurrency = Tree.DefaultProcessCount;
            var results = new ConcurrentBag<EntryCommitInfo>();
            using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(opt.MaxConcurrency))
            {
                List<Task> tasks = new List<Task>();
                
                foreach (var item in Entries)
                {
                    await concurrencySemaphore.WaitAsync();

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var info = new EntryCommitInfo { Entry = item };
                            var ePath = System.IO.Path.Combine(string.IsNullOrEmpty(opt.Path)?"":opt.Path, item.Name);
                            info.Commit = await commit.CommitByPath(new CommitByRevisionOptions { Path = ePath, Timeout = opt.Timeout });
                            if (item.IsCommit()) info.Submodule = await commit.GetSubmodule(ePath);
                            results.Add(info);
                        }
                        finally
                        {
                            concurrencySemaphore.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
                
            }
            return results.ToList();
        }
    }
}