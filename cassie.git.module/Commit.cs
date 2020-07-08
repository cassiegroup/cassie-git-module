using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cassie.git.module.repo;
using cassie.git.module.tree;

namespace cassie.git.module
{
    // Submodule contains information of a Git submodule.
    public class Commit : Tree
    {
        // The SHA-1 hash of the commit.
        public SHA1 ID { get; set; }
        // Author *Signature
        public Signature Author { get; set; }
        // The committer of the commit.
        public Signature Committer { get; set; }
        // The full commit message.
        public string Message { get; set; }

        public List<SHA1> Parents { get; set; }
        public static ConcurrentDictionary<string, object> Submodules = new ConcurrentDictionary<string, object>();

        public Commit()
        {
            this.Parents = new List<SHA1>();
            Submodules = new ConcurrentDictionary<string, object>();
        }

        // Summary returns first line of commit message.
        public string Summary()
        {
            return this.Message.Split('\n').First();
        }

        // ParentsCount returns number of parents of the commit.
        // It returns 0 if this is the root commit, otherwise returns 1, 2, etc.
        public int ParentsCount()
        {
            return this.Parents.Count;
        }

        // ParentID returns the SHA-1 hash of the n-th parent (0-based) of this commit.
        // It returns an ErrParentNotExist if no such parent exists.
        public SHA1 ParentID(int n)
        {
            if (n >= this.Parents.Count)
            {
                throw new Exception("n can not be greater than parents count");
            }

            return this.Parents[n];
        }

        // Parent returns the n-th parent commit (0-based) of this commit.
        // It returns ErrRevisionNotExist if no such parent exists.
        public async Task<Commit> GetParent(int n, params CatFileCommitOptions[] opts)
        {
            var id = ParentID(n);
            return await this.Repo.CatFileCommit(id.String(), opts);
        }
        // CommitByPath returns the commit of the path in the state of this commit.
        public async Task<Commit> CommitByPath(params CommitByRevisionOptions[] opts)
        {
            return await this.Repo.CommitByRevision(this.ID.String(), opts);
        }
        // CommitsByPage returns a paginated list of commits in the state of this commit.
        // The returned list is in reverse chronological order.
        public async Task<List<Commit>> CommitsByPage(int page, int size, params CommitsByPageOptions[] opts)
        {
            return await this.Repo.CommitsByPage(this.ID.String(), page, size, opts);
        }
        // SearchCommits searches commit message with given pattern. The returned list is in reverse
        // chronological order.
        public async Task<List<Commit>> SearchCommits(string pattern, params SearchCommitsOptions[] opts)
        {
            return await this.Repo.SearchCommits(this.ID.String(), pattern, opts);
        }
        // ShowNameStatus returns name status of the commit.
        public async Task<NameStatus> ShowNameStatus(params ShowNameStatusOptions[] opts)
        {
            return await this.Repo.ShowNameStatus(this.ID.String(), opts);
        }
        // CommitsCount returns number of total commits up to this commit.
        public async Task<Int64> CommitsCount(string pattern, params RevListCountOptions[] opts)
        {
            return await this.Repo.RevListCount(new List<string> { this.ID.String() }, opts);
        }
        // FilesChangedAfter returns a list of files changed after given commit ID.
        public async Task<List<string>> FilesChangedAfter(string after, params DiffNameOnlyOptions[] opts)
        {
            return await this.Repo.DiffNameOnly(after, this.ID.String(), opts);
        }
        // CommitsAfter returns a list of commits after given commit ID up to this commit. The returned
        // list is in reverse chronological order.
        public async Task<List<Commit>> CommitsAfter(string after, params RevListOptions[] opts)
        {
            return await this.Repo.RevList(new List<string> { after + "..." + this.ID.String() }, opts);
        }

        // Ancestors returns a list of ancestors of this commit in reverse chronological order.
        public async Task<List<Commit>> Ancestors(params LogOptions[] opts)
        {
            if (this.ParentsCount() == 0) return new List<Commit>();
            var opt = new LogOptions();
            if (opts.Length > 0) opt = opts[0];
            opt.Skip++;
            return await this.Repo.Log(this.ID.String(), opts);
        }
        // IsImageFile returns true if the blob of the commit is an image by subpath.
        public async Task<bool> IsImageFile(string subPath)
        {
            var b = await this.NewBlob(subPath, new LsTreeOptions { Timeout = 60 });
            if (b == null) return false;
            Func<byte[],int> stdOut= s => 0;
            return Utils.GetImageType(b.Pipeline(stdOut))!="";
        }
        // IsImageFileByIndex returns true if the blob of the commit is an image by index.
        public async Task<bool> IsImageFileByIndex(string index)
        {
            var b = await this.BlobByIndex(index);
            if(b == null) return false;
            Func<byte[], int> stdOut = s => 0;
            return Utils.GetImageType(b.Pipeline(stdOut)) != "";
        }
    }
}