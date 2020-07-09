using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using cassie.git.module.commits;
using cassie.git.module.diffs;
using cassie.git.module.hook;
using cassie.git.module.tree;

namespace cassie.git.module.repo
{
    public class Repository
    {
        public string Path { get; set; }
        public static readonly ConcurrentDictionary<string, Commit> CachedCommits = new ConcurrentDictionary<string, Commit>();
        public static readonly ConcurrentDictionary<string, Tag> CachedTags = new ConcurrentDictionary<string, Tag>();

        public const string LogFormatHashOnly = "format:%H";

        public Repository(string path)
        {
            this.Path = path;

        }

        #region private methods

        // parseCommit parses commit information from the (uncompressed) raw data of the commit object.
        // It assumes "\n\n" separates the header from the rest of the message.
        private Commit parseCommit(string data)
        {
            var commit = new Commit();
            var eol = data.IndexOf('\n');
            if (eol == 0)
            {
                commit.Message = data.Substring(1);
                return commit;
            }
            var lines = data.Split('\n');
            if (lines.Length > 1 && string.IsNullOrEmpty(lines[0]))
            {
                commit.Message = lines[1];
                return commit;
            }
            foreach (var line in lines)
            {
                var spacepos = line.IndexOf(' ');
                if (spacepos < 1) continue;
                var reftype = line.Substring(0, spacepos);
                var val = line.Substring(spacepos + 1);
                switch (reftype)
                {
                    case "tree":
                    case "object":
                        var id = SHA1.NewIDFromString(val);
                        commit.TreeID = id;
                        break;
                    case "parent":
                        id = SHA1.NewIDFromString(val);
                        commit.Parents.Add(id);
                        break;
                    case "author":
                    case "tagger":
                        var sig = Signature.ParseSignature(val);
                        commit.Author = sig;
                        break;
                    case "committer":
                        sig = Signature.ParseSignature(val);
                        commit.Committer = sig;
                        break;
                }
            }
            return commit;
        }

        // parseTag parses tag information from the (uncompressed) raw data of the tag object.
        // It assumes "\n\n" separates the header from the rest of the message.
        private Tag parseTag(string data)
        {
            var tag = new Tag();
            var eol = data.IndexOf('\n');
            if (eol == 0)
            {
                tag.Message = data.Substring(1);
                return tag;
            }
            var lines = data.Split('\n');
            if (lines.Length == 0) return tag;
            if (lines.Length > 1 && string.IsNullOrEmpty(lines[0]))
            {
                tag.Message = lines[1];
                return tag;
            }
            foreach (var line in lines)
            {
                var spacepos = line.IndexOf(' ');
                if (spacepos < 1) continue;
                var reftype = line.Substring(0, spacepos);
                var val = line.Substring(spacepos + 1);
                switch (reftype)
                {
                    case "object":
                        var id = SHA1.NewIDFromString(val);
                        tag.CommitID = id;
                        break;
                    case "type":
                    case "tagger":
                        var sig = Signature.ParseSignature(val);
                        tag.Tagger = sig;
                        break;
                }
            }
            return tag;
        }

        // parseTree parses tree information from the (uncompressed) raw data of the tree object.
        private List<TreeEntry> parseTree(Tree tree, string lines)
        {
            var entries = new List<TreeEntry>();
            var list = lines.Split('\n');
            foreach (var item in list)
            {
                if(string.IsNullOrEmpty(item)) continue;
                var fields = Regex.Split(item, @"\s+");
                var entry = new TreeEntry();
                entry.Parent = tree;
                if (fields.Length != 4) throw new Exception("error message format,should 100644 blob 67b72042b5c3ea21777ca62ea731b0cd89a78bbd    .DS_Store");
                switch (fields[0])
                {
                    case "100644":
                    case "100664":
                        entry.Mode = EntryMode.EntryBlob;
                        entry.Typ = ObjectType.ObjectBlob;
                        break;
                    case "100755":
                        entry.Mode = EntryMode.EntryExec;
                        entry.Typ = ObjectType.ObjectBlob;
                        break;
                    case "120000":
                        entry.Mode = EntryMode.EntrySymlink;
                        entry.Typ = ObjectType.ObjectBlob;
                        break;
                    case "160000":
                        entry.Mode = EntryMode.EntryCommit;
                        entry.Typ = ObjectType.ObjectCommit;
                        break;
                    case "040000":
                        entry.Mode = EntryMode.EntryTree;
                        entry.Typ = ObjectType.ObjectTree;
                        break;
                    default:
                        throw new Exception("error message format,should 100644 blob 67b72042b5c3ea21777ca62ea731b0cd89a78bbd    .DS_Store");
                }
                var id = SHA1.NewIDFromString(fields[2]);
                entry.ID = id;
                entry.Name = fields[3];
                entries.Add(entry);
            }
            return entries;
        }
        #endregion
        // public async Task<string> RevParse(string rev, params Int64[] opts)
        // {
        //     Int64 opt = 0;
        //     if (opts != null && opts.Length > 0)
        //         opt = opts[0];
        //     var cmd = new Command("rev-parse", rev);
        //     var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt);
        //     return result.StdOut == "" ? "" : result.StdOut.Trim();
        // }

        // Init initializes a new Git repository.
        public async Task Init(string path, params InitOptions[] opts)
        {
            var opt = new InitOptions();
            if (opts.Length > 0) opt = opts[0];
            try
            {
                Utils.Mkdir(path);
            }
            catch (System.Exception)
            {
                throw;
            }

            var cmd = new Command("init");
            if (opt.Bare) cmd.AddArgs("--bare");
            await cmd.RunAsync(dir: path, timeoutMs: opt.Timeout);
        }


        // Clone clones the repository from remote URL to the destination.
        public async Task Clone(string url, string dst, params CloneOptions[] opts)
        {
            var opt = new CloneOptions();
            if (opts.Length > 0) opt = opts[0];
            var directoryPath = System.IO.Path.GetDirectoryName(dst);
            try
            {
                Utils.Mkdir(directoryPath);
            }
            catch (System.Exception)
            {
                throw;
            }

            var cmd = new Command("clone");
            if (opt.Bare) cmd.AddArgs("--bare");
            if (opt.Mirror) cmd.AddArgs("--mirror");
            if (opt.Quiet) cmd.AddArgs("--quiet");
            if (!opt.Bare && !string.IsNullOrEmpty(opt.Branch)) cmd.AddArgs("-b", opt.Branch);
            cmd.AddArgs(url, dst);
            await cmd.RunAsync(timeoutMs: opt.Timeout);
        }
        // Fetch fetches updates for the repository.
        public async Task Fetch(params FetchOptions[] opts)
        {
            var opt = new FetchOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("fetch");
            if (opt.Prune) cmd.AddArgs("--prune");
            await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
        }

        // Pull pulls updates for the repository.
        public async Task Pull(params PullOptions[] opts)
        {
            var opt = new PullOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("pull");
            if (opt.Rebase) cmd.AddArgs("--rebase");
            if (opt.All) cmd.AddArgs("--all");
            if (!opt.All && !string.IsNullOrEmpty(opt.Remote))
            {
                cmd.AddArgs(opt.Remote);
                if (!string.IsNullOrEmpty(opt.Branch))
                    cmd.AddArgs(opt.Branch);
            }
            await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
        }

        // Push pushs local changes to given remote and branch for the repository
        // in given path.
        public async Task Push(string repoPath, string remote, string branch, params PushOptions[] opts)
        {
            var opt = new PushOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("push", remote, branch);
            if (opt.Envs != null && opt.Envs.Count > 0) cmd.AddEnvs(opt.Envs);
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Push pushs local changes to given remote and branch for the repository.
        public async Task Push(string remote, string branch, params PushOptions[] opts)
        {
            await Push(this.Path, remote, branch, opts);
        }
        // RepoCheckout checks out to given branch for the repository in given path.
        public async Task RepoCheckout(string repoPath, string branch, params CheckoutOptions[] opts)
        {
            var opt = new CheckoutOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("checkout");
            if (!string.IsNullOrEmpty(opt.BaseBranch)) cmd.AddArgs("-b");
            cmd.AddArgs(branch);
            if (!string.IsNullOrEmpty(opt.BaseBranch)) cmd.AddArgs(opt.BaseBranch);
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Checkout checks out to given branch for the repository.
        public async Task Checkout(string branch, params CheckoutOptions[] opts)
        {
            await this.RepoCheckout(this.Path, branch, opts);
        }
        // RepoReset resets working tree to given revision for the repository in given path.
        public async Task RepoReset(string repoPath, string rev, params ResetOptions[] opts)
        {
            var opt = new ResetOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("reset");
            if (opt.Hard) cmd.AddArgs("--hard");
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Reset resets working tree to given revision for the repository.
        public async Task Reset(string rev, params ResetOptions[] opts)
        {
            await this.RepoReset(this.Path, rev, opts);
        }
        // RepoMove moves a file, a directory, or a symlink file or directory from source to
        // destination for the repository in given path.
        public async Task RepoMove(string repoPath, string src, string dst, params MoveOptions[] opts)
        {
            var opt = new MoveOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("mv", src, dst);
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Move moves a file, a directory, or a symlink file or directory from source to destination
        // for the repository.
        public async Task Move(string src, string dst, params MoveOptions[] opts)
        {
            await RepoMove(this.Path, src, dst, opts);
        }
        // RepoAdd adds local changes to index for the repository in given path.
        public async Task RepoAdd(string repoPath, params AddOptions[] opts)
        {
            var opt = new AddOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("add");
            if (opt.All) cmd.AddArgs("--all");
            if (opt.Pathsepcs != null && opt.Pathsepcs.Count > 0)
            {
                cmd.AddArgs("--");
                cmd.AddArgs(opt.Pathsepcs.ToArray());
            }
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Add adds local changes to index for the repository in given path.
        public async Task Add(params AddOptions[] opts)
        {
            await this.RepoAdd(this.Path, opts);
        }
        // RepoCommit commits local changes with given author, committer and message for the
        // repository in given path.
        public async Task RepoCommit(string repoPath, Signature committer, string message, params CommitOptions[] opts)
        {
            var opt = new CommitOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("commit");
            cmd.AddEnvs("GIT_COMMITTER_NAME=" + committer.Name, "GIT_COMMITTER_EMAIL=" + committer.Email);
            if (opt.Author == null) opt.Author = committer;
            cmd.AddArgs($"--author='{opt.Author.Name} <{opt.Author.Email}>'");
            cmd.AddArgs("-m", message);
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }
        // Commit commits local changes with given author, committer and message for the repository.
        public async Task Commit(Signature committer, string message, params CommitOptions[] opts)
        {
            await this.RepoCommit(this.Path, committer, message, opts);
        }
        // RepoShowNameStatus returns name status of given revision of the repository in given path.
        public async Task<NameStatus> RepoShowNameStatus(string repoPath, string rev, params ShowNameStatusOptions[] opts)
        {
            var opt = new ShowNameStatusOptions();
            if (opts.Length > 0) opt = opts[0];
            var fileStatus = new NameStatus();
            var cmd = new Command("show", "--name-status", "--pretty=format:''", rev);
            Action<string> receivedEvent = (item) =>
            {
                if (!item.StartsWith("M") && !item.StartsWith("A") && !item.StartsWith("D")) return;
                var status = item.Trim().Split('\t');
                if (status == null || status.Count() < 2) return;
                switch (status[0])
                {
                    case "M":
                        fileStatus.Modified.Add(status[1]);
                        break;
                    case "A":
                        fileStatus.Added.Add(status[1]);
                        break;
                    case "D":
                        fileStatus.Removed.Add(status[1]);
                        break;
                }
            };
            var result = await cmd.RunAsync(dir: repoPath, stdOut: receivedEvent, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            if (string.IsNullOrEmpty(result.StdOut)) throw new Exception("StdOut can not be null");

            return fileStatus;
        }

        // ShowNameStatus returns name status of given revision of the repository.
        public async Task<NameStatus> ShowNameStatus(string rev, params ShowNameStatusOptions[] opts)
        {
            return await this.RepoShowNameStatus(this.Path, rev, opts);
        }

        // RevParse returns full length (40) commit ID by given revision in the repository.
        public async Task<string> RevParse(string rev, params RevParseOptions[] opts)
        {
            var opt = new RevParseOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("rev-parse", rev);
            var output = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(output.StdErr)) throw new Exception("error occored when executing rev-parse command");
            return output.StdOut.Trim();
        }

        // RepoCountObjects returns disk usage report of the repository in given path.
        public async Task<CountObject> RepoCountObjects(string repoPath, params CountObjectsOptions[] opts)
        {
            var opt = new CountObjectsOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("count-objects", "-v");
            var countObject = new CountObject();
            Action<string> receivedEvent = (item) =>
            {
                if (item.StartsWith("count: ")) countObject.Count = Convert.ToInt64(item.Substring(7));
                if (item.StartsWith("size: ")) countObject.Size = Convert.ToInt64(item.Substring(6)) * 1024;
                if (item.StartsWith("in-pack: ")) countObject.InPack = Convert.ToInt64(item.Substring(9));
                if (item.StartsWith("packs: ")) countObject.Packs = Convert.ToInt64(item.Substring(7));
                if (item.StartsWith("size-pack: ")) countObject.SizePack = Convert.ToInt64(item.Substring(11)) * 1024;
                if (item.StartsWith("prune-packable: ")) countObject.PrunePackable = Convert.ToInt64(item.Substring(16));
                if (item.StartsWith("garbage: ")) countObject.Garbage = Convert.ToInt64(item.Substring(9));
                if (item.StartsWith("size-garbage: ")) countObject.SizeGarbage = Convert.ToInt64(item.Substring(14)) * 1024;
            };

            var result = await cmd.RunAsync(dir: repoPath, stdOut: receivedEvent, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return countObject;
        }

        // CountObjects returns disk usage report of the repository.
        public async Task<CountObject> CountObjects(params CountObjectsOptions[] opts)
        {
            return await this.RepoCountObjects(this.Path, opts);
        }
        // RepoFsck verifies the connectivity and validity of the objects in the database for the
        // repository in given path.
        public async Task RepoFsck(string repoPath, params FsckOptions[] opts)
        {
            var opt = new FsckOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("fsck");
            if (opt.Args.Count > 0)
                cmd.AddArgs(opt.Args.ToArray());
            await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
        }

        // Fsck verifies the connectivity and validity of the objects in the database for the repository.
        public async Task Fsck(params FsckOptions[] opts)
        {
            await this.RepoFsck(this.Path, opts);
        }

        // parsePrettyFormatLogToList returns a list of commits parsed from given logs that are
        // formatted in LogFormatHashOnly.
        public async Task<List<Commit>> ParsePrettyFormatLogToList(Int64 timeout, string logs)
        {
            if (logs.Length == 0) return new List<Commit>();
            var ids = logs.Split('\n');
            var list = new List<Commit>();
            foreach (var id in ids)
            {
                if(string.IsNullOrEmpty(id)) continue;
                var commit = await this.CatFileCommit(id, new CatFileCommitOptions { Timeout = timeout });
                list.Add(commit);
            }
            return list;
        }

        // CatFileCommit returns the commit corresponding to the given revision of the repository.
        // The revision could be a commit ID or full refspec (e.g. "refs/heads/master").
        public async Task<Commit> CatFileCommit(string rev, params CatFileCommitOptions[] opts)
        {
            CatFileCommitOptions opt = new CatFileCommitOptions();
            if (opts != null && opts.Length > 0) opt = opts[0];
            Commit cacheCommit = null;
            if (CachedCommits.Keys.Contains(rev)) cacheCommit = CachedCommits[rev];
            if (null != cacheCommit) return cacheCommit;
            string commitID = "";
            if (opt.Timeout == 0) commitID = await this.RevParse(rev, new RevParseOptions{ Timeout= Command.DefaultTimeout});
            else commitID = await this.RevParse(rev, new RevParseOptions { Timeout = opt.Timeout });
            var cmd = new Command("cat-file", "commit", commitID);
            var result = await cmd.RunAsync(dir: this.Path, splitChar: '\n', timeoutMs: opt.Timeout);
            if (result == null) throw new Exception("result can not be null");
            var c = this.parseCommit(result.StdOut);
            if (c == null) throw new Exception("commit can not be null");
            c.Repo = this;
            c.ID = SHA1.MustIDFromString(commitID);
            CachedCommits.TryAdd(commitID, c);
            return c;
        }
        // CatFileType returns the object type of given revision of the repository.
        public async Task<ObjectType?> CatFileType(string rev, params CatFileTypeOptions[] opts)
        {
            CatFileTypeOptions opt = new CatFileTypeOptions();
            if (opts != null && opts.Length > 0) opt = opts[0];
            var cmd = new Command("cat-file", "-t", rev);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var ot = result.StdOut.Trim();
            return ObjectTypeOpt.ToObjectType(ot);
        }
        // BranchCommit returns the latest commit of given branch of the repository.
        // The branch must be given in short name e.g. "master".
        public async Task<Commit> BranchCommit(string branch, params CatFileCommitOptions[] opts)
        {
            return await this.CatFileCommit(RepositoryConst.RefsHeads + branch, opts);
        }
        // TagCommit returns the latest commit of given tag of the repository.
        // The tag must be given in short name e.g. "v1.0.0".
        public async Task<Commit> TagCommit(string tag, params CatFileCommitOptions[] opts)
        {
            return await this.CatFileCommit(RepositoryConst.RefsTags + tag, opts);
        }

        // Open opens the repository at the given path. It throw an exception
        // if the path does not exist.
        public Repository Open(string repoPath)
        {
            var absolute = System.IO.Path.IsPathRooted(repoPath);
            if (!absolute) repoPath = System.IO.Path.GetFullPath(repoPath);
            var attributes = File.GetAttributes(repoPath);
            if (!attributes.HasFlag(FileAttributes.Directory))
                throw new Exception("path must be the directory");
            var repo = new Repository(repoPath);
            return repo;
        }
        // RepoLog returns a list of commits in the state of given revision of the repository
        // in given path. The returned list is in reverse chronological order.
        public async Task<List<Commit>> RepoLog(string repoPath, string rev, params LogOptions[] opts)
        {
            var opt = new LogOptions();
            if (opts.Length > 0) opt = opts[0];
            var r = this.Open(repoPath);
            return await r.Log(rev, opts);
        }

        // Log returns a list of commits in the state of given revision of the repository.
        // The returned list is in reverse chronological order.
        public async Task<List<Commit>> Log(string rev, params LogOptions[] opts)
        {
            var opt = new LogOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("log", "--pretty=" + LogFormatHashOnly, rev);
            if (opt.MaxCount > 0) cmd.AddArgs("--max-count=" + opt.MaxCount);
            if (opt.Skip > 0) cmd.AddArgs("--skip=" + opt.Skip);
            if (opt.Since != null) cmd.AddArgs("--skip=" + Utils.ToRfc3339String(opt.Since));
            if (!string.IsNullOrEmpty(opt.GrepPattern)) cmd.AddArgs("--grep=" + opt.GrepPattern);
            if (opt.RegexpIgnoreCase) cmd.AddArgs("--regexp-ignore-case");
            cmd.AddArgs("--");
            if (!string.IsNullOrEmpty(opt.Path)) cmd.AddArgs(Utils.EscapePath(opt.Path));
            var result = await cmd.RunAsync(dir: this.Path, splitChar: '\n', timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return await this.ParsePrettyFormatLogToList(opt.Timeout, result.StdOut);
        }
        // CommitByRevisionOptions returns a commit by given revision.
        public async Task<Commit> CommitByRevision(string rev, params CommitByRevisionOptions[] opts)
        {
            var opt = new CommitByRevisionOptions();
            if (opts.Length > 0) opt = opts[0];
            var commits = await this.Log(rev, new LogOptions
            {
                MaxCount = 1,
                Path = opt.Path,
                Timeout = opt.Timeout
            });
            if (commits == null || commits.Count == 0) throw new Exception("commits can not be null");
            return commits[0];
        }
        // CommitsByPage returns a paginated list of commits in the state of given revision.
        // The pagination starts from the newest to the oldest commit.
        public async Task<List<Commit>> CommitsByPage(string rev, int page, int size, params CommitsByPageOptions[] opts)
        {
            var opt = new CommitsByPageOptions();
            if (opts.Length > 0) opt = opts[0];
            return await this.Log(rev, new LogOptions
            {
                MaxCount = size,
                Skip = (page - 1) * size,
                Path = opt.Path,
                Timeout = opt.Timeout
            });
        }

        // SearchCommits searches commit message with given pattern in the state of given revision.
        // The returned list is in reverse chronological order.
        public async Task<List<Commit>> SearchCommits(string rev, string pattern, params SearchCommitsOptions[] opts)
        {
            var opt = new SearchCommitsOptions();
            if (opts.Length > 0) opt = opts[0];
            return await this.Log(rev, new LogOptions
            {
                MaxCount = opt.MaxCount,
                GrepPattern = pattern,
                RegexpIgnoreCase = true,
                Path = opt.Path,
                Timeout = opt.Timeout
            });
        }

        // CommitsSince returns a list of commits since given time. The returned list is in reverse
        // chronological order.
        public async Task<List<Commit>> CommitsSince(string rev, DateTime since, params CommitsSinceOptions[] opts)
        {
            var opt = new CommitsSinceOptions();
            if (opts.Length > 0) opt = opts[0];
            return await this.Log(rev, new LogOptions
            {
                Since = since,
                Path = opt.Path,
                Timeout = opt.Timeout
            });
        }
        // RepoDiffNameOnly returns a list of changed files between base and head revisions of
        // the repository in given path.
        public async Task<List<string>> RepoDiffNameOnly(string repoPath, string baseBranch, string head, params DiffNameOnlyOptions[] opts)
        {
            var opt = new DiffNameOnlyOptions();
            if (opts.Length > 0) opt = opts[0];

            var cmd = new Command("diff", "--name-only");
            if (opt.NeedsMergeBase) cmd.AddArgs(baseBranch + "..." + head);
            else cmd.AddArgs(baseBranch, head);
            cmd.AddArgs("--");
            if (!string.IsNullOrEmpty(opt.Path)) cmd.AddArgs(Utils.EscapePath(opt.Path));
            var names = new List<string>();
            Action<string> receivedEvent = (item) =>
            {
                if (item.Length > 0) names.Add(item);
            };
            var result = await cmd.RunAsync(dir: this.Path, stdOut: receivedEvent, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return names;
        }
        // DiffNameOnly returns a list of changed files between base and head revisions of the
        // repository.
        public async Task<List<string>> DiffNameOnly(string baseBranch, string head, params DiffNameOnlyOptions[] opts)
        {
            return await this.RepoDiffNameOnly(this.Path, baseBranch, head, opts);
        }
        // RevListCount returns number of total commits up to given refspec of the repository.
        public async Task<Int64> RevListCount(List<string> refspecs, params RevListCountOptions[] opts)
        {
            var opt = new RevListCountOptions();
            if (opts.Length > 0) opt = opts[0];
            if(refspecs == null || refspecs.Count == 0) throw new Exception("refspecs list can not be null");
            var cmd = new Command("rev-list", "--count");
            cmd.AddArgs(refspecs.ToArray());
            cmd.AddArgs("--");
            if(!string.IsNullOrEmpty(opt.Path)) cmd.AddArgs(Utils.EscapePath(opt.Path));
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return Convert.ToInt64(result.StdOut);
        }

        // RevList returns a list of commits based on given refspecs in reverse chronological order.
        public async Task<List<Commit>> RevList(List<string> refspecs, params RevListOptions[] opts)
        {
            var opt = new RevListOptions();
            if (opts.Length > 0) opt = opts[0];
            if (refspecs == null || refspecs.Count == 0) throw new Exception("refspecs list can not be null");
            var cmd = new Command("rev-list");
            cmd.AddArgs(refspecs.ToArray());
            cmd.AddArgs("--");
            if (!string.IsNullOrEmpty(opt.Path)) cmd.AddArgs(Utils.EscapePath(opt.Path));
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout,splitChar:'\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return await this.ParsePrettyFormatLogToList(opt.Timeout,result.StdOut);
        }
        // LatestCommitTime returns the time of latest commit of the repository.
        public async Task<DateTime> LatestCommitTime(params LatestCommitTimeOptions[] opts)
        {
            var opt = new LatestCommitTimeOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("for-each-ref",
                                    "--count=1",
                                    "--sort=-committerdate",
                                    "--format=%(committerdate:iso8601)");

            if (!string.IsNullOrEmpty(opt.Branch)) cmd.AddArgs(RepositoryConst.RefsHeads + opt.Branch);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return Convert.ToDateTime(result.StdOut.Trim());
        }
        // Diff returns a parsed diff object between given commits of the repository.
        public async Task<Diff> Diff(string rev, int maxFiles, int maxFileLines, int maxLineChars, params DiffOptions[] opts)
        {
            var opt = new DiffOptions();
            if (opts.Length > 0) opt = opts[0];
            var cfco = new CatFileCommitOptions { Timeout = opt.Timeout };
            var commit = await this.CatFileCommit(rev, cfco);
            var cmd = new Command();
            if (string.IsNullOrEmpty(opt.Base))
            {
                // First commit of repository
                if (commit.ParentsCount() == 0) cmd.AddArgs("show", "--full-index", rev);
                else
                {
                    var c = await commit.GetParent(0, cfco);
                    cmd.AddArgs("diff", "--full-index", "-M", c.ID.String(), rev);
                }
            }
            else
            {
                cmd.AddArgs("diff", "--full-index", "-M", opt.Base, rev);
            }
            var splitChar = '\n';
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout, splitChar: splitChar);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var dp = new DiffParser(maxFiles, maxFileLines, maxLineChars);
            return dp.StreamParseDiff(result.StdOut, splitChar);
        }
        // RawDiff dumps diff of repository in given revision directly to given io.Writer.
        public async Task RawDiff(string rev, RawDiffFormat diffType, Action<string> received, params RawDiffOptions[] opts)
        {
            var opt = new RawDiffOptions();
            if (opts.Length > 0) opt = opts[0];
            var commit = await CatFileCommit(rev, new CatFileCommitOptions { Timeout = opt.Timeout });
            var cmd = new Command();
            switch (diffType)
            {
                case RawDiffFormat.RawDiffNormal:
                    if (commit.ParentsCount() == 0) cmd.AddArgs("show", rev);
                    else
                    {
                        var c = await commit.GetParent(0, new CatFileCommitOptions { Timeout = opt.Timeout });
                        cmd.AddArgs("diff", "-M", c.ID.String(), rev);
                    }
                    break;
                case RawDiffFormat.RawDiffPatch:
                    if (commit.ParentsCount() == 0) cmd.AddArgs("format-patch", "--no-signature", "--stdout", "--root", rev);
                    else
                    {
                        var c = await commit.GetParent(0, new CatFileCommitOptions { Timeout = opt.Timeout });
                        cmd.AddArgs("format-patch", "--no-signature", "--stdout", rev + "..." + c.ID.String());
                    }
                    break;
                default:
                    throw new Exception("invalid diffType:" + diffType.ToTypeString());
            }
            var result = await cmd.RunAsync(timeoutMs: opt.Timeout, dir: this.Path, splitChar: '\n', stdOut: received);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
        }
        // DiffBinary returns binary patch between base and head revisions that could be used for git-apply.
        public async Task<string> DiffBinary(string b, string head, params DiffBinaryOptions[] opts)
        {
            var opt = new DiffBinaryOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("diff", "--binary", b, head);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout,splitChar:'\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return result.StdOut;
        }

        // NewHook creates and returns a new hook with given name. Update method must be called
        // to actually save the hook to disk.
        public Hook NewHook(string dir, HookName name)
        {
            var hook = new Hook(System.IO.Path.Combine(this.Path, dir, name.ToTypeString()), name);
            return hook;
        }

        // Hook returns a Git hook by given name in the repository. Giving empty directory
        // will use the default directory. It returns an os.ErrNotExist if both active and
        // sample hook do not exist.
        public Hook Hook(string dir, HookName name)
        {
            if (string.IsNullOrEmpty(dir)) dir = HookConst.DefaultHooksDir;
            // 1. Check if there is an active hook.
            string fPath = System.IO.Path.Combine(this.Path, dir, name.ToTypeString());
            if (Utils.IsFile(fPath))
            {
                var hook = new Hook
                {
                    Name = name,
                    Path = fPath,
                    Content = File.ReadAllText(fPath)
                };
                return hook;
            }
            else
            {
                var sample = ServerSideHook.ServerSideHookSamples[name];
                if (string.IsNullOrEmpty(sample)) throw new Exception("sample content can not be null");
                var hook = new Hook
                {
                    Name = name,
                    Path = fPath,
                    Content = sample,
                    IsSample = true
                };
                return hook;
            }
        }
        // Hooks returns a list of Git hooks found in the repository. Giving empty directory
        // will use the default directory. It may return an empty slice when no hooks found.
        public List<Hook> Hooks(string dir)
        {
            var hooks = new List<Hook>();
            foreach (HookName name in (HookName[])Enum.GetValues(typeof(HookName)))
            {
                var hook = this.Hook(dir, name);
                hooks.Add(hook);
            }
            return hooks;
        }

        // RepoMergeBase returns merge base between base and head revisions of the repository
        // in given path.
        public async Task<string> RepoMergeBase(string repoPath, string b, string head, params MergeBaseOptions[] opts)
        {
            var opt = new MergeBaseOptions();
            if (opts.Length > 0) opt = opts[0];

            var cmd = new Command("merge-base", b, head);
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return result.StdOut.Trim();
        }

        // MergeBase returns merge base between base and head revisions of the repository.
        public async Task<string> MergeBase(string b, string head, params MergeBaseOptions[] opts)
        {
            return await this.RepoMergeBase(this.Path, b, head, opts);
        }

        // RefShortName returns short name of heads or tags. Other references will retrun original string.
        public string RefShortName(string refname)
        {
            if (refname.StartsWith(RepositoryConst.RefsHeads)) return refname.Substring(RepositoryConst.RefsHeads.Length);
            else if (refname.StartsWith(RepositoryConst.RefsTags)) return refname.Substring(RepositoryConst.RefsTags.Length);
            return refname;
        }
        // RepoShowRefVerify returns the commit ID of given reference if it exists in the repository
        // in given path.
        public async Task<string> RepoShowRefVerify(string repoPath, string refname, params ShowRefVerifyOptions[] opts)
        {
            var opt = new ShowRefVerifyOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("show-ref", "--verify", refname);
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var refs = result.StdOut.Split(' ');
            if (refs.Length > 0) return refs.First();
            throw new Exception("refs length must be greatter than 0");
        }
        // ShowRefVerify returns the commit ID of given reference (e.g. "refs/heads/master")
        // if it exists in the repository.
        public async Task<string> ShowRefVerify(string refname, params ShowRefVerifyOptions[] opts)
        {
            return await this.RepoShowRefVerify(this.Path, refname, opts);
        }
        // BranchCommitID returns the commit ID of given branch if it exists in the repository.
        // The branch must be given in short name e.g. "master".
        public async Task<string> BranchCommitID(string branch, params ShowRefVerifyOptions[] opts)
        {
            return await this.ShowRefVerify(this.Path + branch, opts);
        }
        // TagCommitID returns the commit ID of given tag if it exists in the repository.
        // The tag must be given in short name e.g. "v1.0.0".
        public async Task<string> TagCommitID(string tag, params ShowRefVerifyOptions[] opts)
        {
            return await this.ShowRefVerify(this.Path + tag, opts);
        }
        // RepoHasReference returns true if given reference exists in the repository in given path.
        // The reference must be given in full refspec, e.g. "refs/heads/master".
        public async Task<bool> RepoHasReference(string repoPath, string refname, params ShowRefVerifyOptions[] opts)
        {
            try
            {
                await this.RepoShowRefVerify(repoPath, refname, opts);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        // RepoHasBranch returns true if given branch exists in the repository in given path.
        // The branch must be given in short name e.g. "master".
        public async Task<bool> RepoHasBranch(string repoPath, string branch, params ShowRefVerifyOptions[] opts)
        {
            return await RepoHasReference(repoPath, RepositoryConst.RefsHeads + branch, opts);
        }
        // RepoHasTag returns true if given tag exists in the repository in given path.
        // The tag must be given in short name e.g. "v1.0.0".
        public async Task<bool> RepoHasTag(string repoPath, string tag, params ShowRefVerifyOptions[] opts)
        {
            return await RepoHasReference(repoPath, RepositoryConst.RefsTags + tag, opts);
        }
        // HasReference returns true if given reference exists in the repository.
        // The reference must be given in full refspec, e.g. "refs/heads/master".
        public async Task<bool> HasReference(string refname, params ShowRefVerifyOptions[] opts)
        {
            return await RepoHasReference(this.Path, refname, opts);
        }
        // HasBranch returns true if given branch exists in the repository.
        // The branch must be given in short name e.g. "master".
        public async Task<bool> HasBranch(string branch, params ShowRefVerifyOptions[] opts)
        {
            return await RepoHasBranch(this.Path, branch, opts);
        }
        // HasTag returns true if given tag exists in the repository.
        // The tag must be given in short name e.g. "v1.0.0".
        public async Task<bool> HasTag(string tag, params ShowRefVerifyOptions[] opts)
        {
            return await RepoHasTag(this.Path, tag, opts);
        }
        // SymbolicRef returns the reference name (e.g. "refs/heads/master") pointed by the
        // symbolic ref. It returns an empty string and nil error when doing set operation.
        public async Task<string> SymbolicRef(params SymbolicRefOptions[] opts)
        {
            var opt = new SymbolicRefOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("symbolic-ref");
            if (string.IsNullOrEmpty(opt.Name)) opt.Name = "HEAD";
            cmd.AddArgs(opt.Name);
            if (!string.IsNullOrEmpty(opt.Ref)) cmd.AddArgs(opt.Ref);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return result.StdOut.Trim();
        }
        // ShowRef returns a list of references in the repository.
        public async Task<List<Reference>> ShowRef(params ShowRefOptions[] opts)
        {
            var opt = new ShowRefOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("show-ref");
            if (opt.Heads) cmd.AddArgs("--heads");
            if (opt.Tags) cmd.AddArgs("--tags");
            cmd.AddArgs("--");
            if (opt.Patterns.Count > 0) cmd.AddArgs(opt.Patterns.ToArray());
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout, splitChar: '\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var lines = result.StdOut.Split('\n');
            var refs = new List<Reference>();
            foreach (var item in lines)
            {
                var fields = Regex.Split(item, @"\s+");
                if (fields.Length < 2) continue;
                refs.Add(new Reference
                {
                    ID = fields.First(),
                    Refspec = fields.Last()
                });
            }
            return refs;
        }
        // Branches returns a list of all branches in the repository.
        public async Task<List<string>> Branches()
        {
            var heads = await this.ShowRef(new ShowRefOptions { Heads = true });
            var branches = new List<string>();
            foreach (var item in heads)
            {
                branches.Add(item.Refspec.Replace(RepositoryConst.RefsHeads, ""));
            }
            return branches;
        }
        // RepoDeleteBranch deletes the branch from the repository in given path.
        public async Task RepoDeleteBranch(string repoPath, string name, params DeleteBranchOptions[] opts)
        {
            var opt = new DeleteBranchOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("branch");
            if (opt.Force) cmd.AddArgs("-D");
            else cmd.AddArgs("-d");
            cmd.AddArgs(name);
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
        }
        // LsRemote returns a list references in the remote repository.
        public async Task<List<Reference>> LsRemote(string url, params LsRemoteOptions[] opts)
        {
            var opt = new LsRemoteOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("ls-remote", "--quiet");
            if (opt.Heads) cmd.AddArgs("--heads");
            if (opt.Tags) cmd.AddArgs("--tags");
            if (opt.Refs) cmd.AddArgs("--refs");
            cmd.AddArgs(url);
            if (opt.Patterns.Count > 0) cmd.AddArgs(opt.Patterns.ToArray());
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout, splitChar: '\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var lines = result.StdOut.Split('\n');
            var refs = new List<Reference>();
            foreach (var item in lines)
            {
                var fields = Regex.Split(item, @"\s+");
                if (fields.Length < 2) continue;
                refs.Add(new Reference
                {
                    ID = fields.First(),
                    Refspec = fields.Last()
                });
            }
            return refs;
        }
        // IsURLAccessible returns true if given remote URL is accessible via Git
        // within given timeout.
        public async Task<bool> IsURLAccessible(string url, Int64 timeout)
        {
            try
            {
                await this.LsRemote(url, new LsRemoteOptions
                {
                    Patterns = new List<string> { "HEAD" },
                    Timeout = timeout
                });
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        // RepoAddRemote adds a new remote to the repository in given path.
        public async Task RepoAddRemote(string repoPath, string name, string url, params AddRemoteOptions[] opts)
        {
            var opt = new AddRemoteOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("remote", "add");
            if (opt.Fetch) cmd.AddArgs("-f");
            if (opt.MirrorFetch) cmd.AddArgs("--mirror=fetch");
            cmd.AddArgs(name, url);
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout, splitChar: '\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);

        }
        // AddRemote adds a new remote to the repository.
        public async Task AddRemote(string name, string url, params AddRemoteOptions[] opts)
        {
            await this.RepoAddRemote(this.Path, name, url, opts);
        }
        // RepoRemoveRemote removes a remote from the repository in given path.
        public async Task RepoRemoveRemote(string repoPath, string name, params RemoveRemoteOptions[] opts)
        {
            var opt = new RemoveRemoteOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("remote", "remove", name);
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
        }
        // RemoveRemote removes a remote from the repository.
        public async Task RemoveRemote(string name, params RemoveRemoteOptions[] opts)
        {
            await this.RepoRemoveRemote(this.Path, name, opts);
        }
        // getTag returns a tag by given SHA1 hash.
        public async Task<Tag> GetTag(SHA1 id, Int64 timeout)
        {
            var t = new Tag();
            if (CachedTags.Keys.Contains(id.String()))
            {
                t = CachedTags[id.String()];
                return t;
            }
            // Check tag type
            var typ = await this.CatFileType(id.String(), new CatFileTypeOptions { Timeout = timeout });
            if (typ == null) throw new Exception("ObjectType can not be null");
            var tag = new Tag();
            switch (typ)
            {
                case ObjectType.ObjectCommit:
                    tag = new Tag
                    {
                        ID = id,
                        Typ = ObjectType.ObjectCommit,
                        CommitID = id,
                        Repo = this
                    };
                    break;
                case ObjectType.ObjectTag:
                    var cmd = new Command("cat-file", "-p", id.String());
                    var result = await cmd.RunAsync(dir: this.Path);
                    if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
                    tag = parseTag(result.StdOut);
                    tag.Typ = ObjectType.ObjectTag;
                    tag.ID = id;
                    tag.Repo = this;
                    break;
                default:
                    throw new Exception($"unsupported tag type: {typ.ToString()}");
            }
            var res = CachedTags.TryAdd(id.String(), tag);
            if (!res) throw new Exception("Dictionary can't insert");

            return tag;
        }
        // Tag returns a Git tag by given name, e.g. "v1.0.0".
        public async Task<Tag> Tag(string name, params TagOptions[] opts)
        {
            var opt = new TagOptions();
            if (opts.Length > 0) opt = opts[0];
            var refspec = RepositoryConst.RefsTags + name;
            var refs = await this.ShowRef(new ShowRefOptions
            {
                Tags = true,
                Patterns = new List<string> { refspec },
                Timeout = opt.Timeout
            });
            if (refs == null || refs.Count == 0) throw new Exception("Refs can not be null");
            var id = SHA1.NewIDFromString(refs.First().ID);
            var tag = await this.GetTag(id, opt.Timeout);
            tag.Refspec = refspec;
            return tag;

        }
        // RepoTags returns a list of tags of the repository in given path.
        public async Task<List<string>> RepoTags(string repoPath, params TagsOptions[] opts)
        {
            var opt = new TagsOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("tag", "--list");
            var ver = await Git.BinVersion();
            var version = ver.CompareTo(new Version("2.4.9"));
            if (version >= 1) cmd.AddArgs("--sort=-creatordate");
            var result = await cmd.RunAsync(dir: repoPath, timeoutMs: opt.Timeout, splitChar: '\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var t = result.StdOut.Split('\n');
            try
            {
                var tags = t.ToList().GetRange(0, t.Length - 1).OrderByDescending(b => b);
                return tags.ToList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }


        }
        // Tags returns a list of tags of the repository.
        public async Task<List<string>> Tags(params TagsOptions[] opts)
        {
            return await this.RepoTags(this.Path, opts);
        }
        // CreateTag creates a new tag on given revision.
        public async Task CreateTag(string name, string rev, params CreateTagOptions[] opts)
        {
            var opt = new CreateTagOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("tag", name, rev);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
        }
        // DeleteTag deletes a tag from the repository.
        public async Task DeleteTag(string name, params DeleteTagOptions[] opts)
        {
            var opt = new DeleteTagOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("tag", "--delete", name);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
        }
        // LsTree returns the tree object in the repository by given revision.
        public async Task<Tree> LsTree(string rev, params LsTreeOptions[] opts)
        {
            var opt = new LsTreeOptions();
            if (opts.Length > 0) opt = opts[0];
            if(opt.Timeout>0) rev = await this.RevParse(rev, new RevParseOptions{Timeout=opt.Timeout});
            else rev = await this.RevParse(rev);
            var t = new Tree
            {
                TreeID=SHA1.MustIDFromString(rev),
                Repo=this
            };
            var cmd = new Command("ls-tree", rev);
            var result = await cmd.RunAsync(dir:this.Path,timeoutMs:opt.Timeout,splitChar:'\n');
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            t.Entries = this.parseTree(t,result.StdOut).OrderBy(a => a.Name).OrderBy(a => a.Mode).ToList();
            return t;
        }
    }

}