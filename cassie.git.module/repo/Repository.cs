using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using cassie.git.module.diffs;

namespace cassie.git.module.repo
{
    public class Repository
    {
        public string Path { get; set; }
        public ConcurrentDictionary<string, Commit> CachedCommits;
        public ConcurrentDictionary<string, object> CachedTags;

        public const string LogFormatHashOnly = "format:%H";

        public Repository()
        {
            this.CachedCommits = new ConcurrentDictionary<string, Commit>();
            this.CachedTags = new ConcurrentDictionary<string, object>();
        }

        public Repository(string path):this()
        {
            this.Path = path;
            
        }
        public async Task<string> RevParse(string rev, params Int64[] opts)
        {
            Int64 opt = 0;
            if (opts != null && opts.Length > 0)
                opt = opts[0];
            var cmd = new Command("rev-parse", rev);
            var result = await cmd.RunAsync(dir: this.Path, timeoutMs: opt);
            return result.StdOut == "" ? "" : result.StdOut.Trim();
        }

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
            if (this.CachedCommits.Keys.Contains(rev)) cacheCommit = this.CachedCommits[rev];
            if (null != cacheCommit) return cacheCommit;
            var commitID = await this.RevParse(rev, opt.Timeout);
            var cmd = new Command("cat-file", "commit", commitID);
            var result = await cmd.RunAsync(dir: this.Path,splitChar:'\n', timeoutMs: opt.Timeout);
            if (result == null) throw new Exception("result can not be null");
            var c = this.parseCommit(result.StdOut);
            if (c == null) throw new Exception("commit can not be null");
            c.Tree.Repo = this;
            c.ID = SHA1.MustIDFromString(commitID);
            this.CachedCommits.TryAdd(commitID, c);
            return c;
        }
        // CatFileType returns the object type of given revision of the repository.
        public async Task<ObjectType?> CatFileType(string rev, params CatFileTypeOptions[] opts)
        {
            CatFileTypeOptions opt = null;
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
            var result = await cmd.RunAsync(dir: this.Path,splitChar:'\n', timeoutMs: opt.Timeout);
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
        // LatestCommitTime returns the time of latest commit of the repository.
        public async Task<DateTime> LatestCommitTime(params LatestCommitTimeOptions[] opts)
        {
            var opt = new LatestCommitTimeOptions();
            if (opts.Length > 0) opt = opts[0];
            var cmd = new Command("for-each-ref",
                                    "--count=1",
                                    "--sort=-committerdate",
                                    "--format=%(committerdate:iso8601)");

            if(!string.IsNullOrEmpty(opt.Branch)) cmd.AddArgs(RepositoryConst.RefsHeads+opt.Branch);
            var result = await cmd.RunAsync(dir:this.Path,timeoutMs:opt.Timeout);
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return Convert.ToDateTime(result.StdOut.Trim());
        }
        // Diff returns a parsed diff object between given commits of the repository.
        public async Task<Diff> Diff(string rev,int maxFiles,int maxFileLines,int maxLineChars,params DiffOptions[] opts)
        {
            var opt = new DiffOptions();
            if (opts.Length > 0) opt = opts[0];
            var commit = await this.CatFileCommit(rev,new CatFileCommitOptions{Timeout=opt.Timeout});
            var cmd = new Command();
            if(string.IsNullOrEmpty(opt.Base))
            {
                // First commit of repository
                if (commit.ParentsCount() ==0) cmd.AddArgs("show", "--full-index", rev);
                else 
                {
                    var c = await commit.Parent(0);
                    cmd.AddArgs("diff", "--full-index", "-M", c.ID.String(), rev);
                }
            }
            else
            { 
                cmd.AddArgs("diff", "--full-index", "-M", opt.Base, rev);
            }
            var splitChar = '\n';
            var result = await cmd.RunAsync(dir:this.Path,timeoutMs:opt.Timeout,splitChar:splitChar);
            if(!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var dp = new DiffParser(maxFiles,maxFileLines,maxLineChars);
            return dp.StreamParseDiff(result.StdOut,splitChar);
        }   

        #region private methods

        // parseCommit parses commit information from the (uncompressed) raw data of the commit object.
        // It assumes "\n\n" separates the header from the rest of the message.
        private Commit parseCommit(string data)
        {
            var commit = new Commit();
            var lines = data.Split('\n');
            if (lines.Length == 0) return commit;
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
                var val = line.Substring(spacepos+1);
                switch (reftype)
                {
                    case "tree":
                    case "object":
                        var id = SHA1.NewIDFromString(val);
                        commit.Tree = new Tree { ID = id };
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

        #endregion
    }
}