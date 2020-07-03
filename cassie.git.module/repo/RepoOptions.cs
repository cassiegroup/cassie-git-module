using System;
using System.Collections.Generic;

namespace cassie.git.module.repo
{
    public class InitOptions
    {
        public bool Bare { get; set; }
        public Int64 Timeout { get; set; }
    }

    // CloneOptions contains optional arguments for cloning a repository.
    // Docs: https://git-scm.com/docs/git-clone
    public class CloneOptions
    {
        // Indicates whether the repository should be cloned as a mirror.
        public bool Mirror { get; set; }
        // Indicates whether the repository should be cloned in bare format.
        public bool Bare { get; set; }
        // Indicates whether to suppress the log output.
        public bool Quiet { get; set; }
        // The branch to checkout for the working tree when Bare=false.
        public string Branch { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // FetchOptions contains optional arguments for fetching repository updates.
    // Docs: https://git-scm.com/docs/git-fetch
    public class FetchOptions
    {
        // Indicates whether to prune during fetching.
        public bool Prune { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // PullOptions contains optional arguments for pulling repository updates.
    // Docs: https://git-scm.com/docs/git-pull
    public class PullOptions
    {

        // Indicates whether to rebased during pulling.
        public bool Rebase { get; set; }
        // Indicates whether to pull from all remotes.
        public bool All { get; set; }
        // The remote to pull updates from when All=false.
        public string Remote { get; set; }
        // The branch to pull updates from when All=false and Remote is supplied.
        public string Branch { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }

    // PushOptions contains optional arguments for pushing repository changes.
    // Docs: https://git-scm.com/docs/git-push
    public class PushOptions
    {
        // The environment variables set for the push.
        public Dictionary<string, string> Envs { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CheckoutOptions contains optional arguments for checking out to a branch.
    // Docs: https://git-scm.com/docs/git-checkout
    public class CheckoutOptions
    {
        // The base branch if checks out to a new branch.
        public string BaseBranch { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // ResetOptions contains optional arguments for resetting a branch.
    // Docs: https://git-scm.com/docs/git-reset
    public class ResetOptions
    {
        // Indicates whether to perform a hard reset.
        public bool Hard { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // MoveOptions contains optional arguments for moving a file, a directory, or a symlink.
    // Docs: https://git-scm.com/docs/git-mv
    public class MoveOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // AddOptions contains optional arguments for adding local changes.
    // Docs: https://git-scm.com/docs/git-add
    public class AddOptions
    {
        // Indicates whether to add all changes to index.
        public bool All { get; set; }
        // The specific pathspecs to be added to index.
        public List<string> Pathsepcs { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }

    // CommitOptions contains optional arguments to commit changes.
    // Docs: https://git-scm.com/docs/git-commit
    public class CatFileCommitOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CommitOptions contains optional arguments to commit changes.
    // Docs: https://git-scm.com/docs/git-commit
    public class CommitOptions
    {
        // Author is the author of the changes if that's not the same as committer.
        public Signature Author { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // NameStatus contains name status of a commit.
    public class NameStatus
    {
        public NameStatus()
        {
            this.Added = new List<string>();
            this.Removed = new List<string>();
            this.Modified = new List<string>();
        }
        public List<string> Added { get; set; }
        public List<string> Removed { get; set; }
        public List<string> Modified { get; set; }
    }
    public class ShowNameStatusOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // RevParseOptions contains optional arguments for parsing revision.
    // Docs: https://git-scm.com/docs/git-rev-parse
    public class RevParseOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CountObject contains disk usage report of a repository.
    public class CountObject
    {
        public Int64 Count { get; set; }
        public Int64 Size { get; set; }
        public Int64 InPack { get; set; }
        public Int64 Packs { get; set; }
        public Int64 SizePack { get; set; }
        public Int64 PrunePackable { get; set; }
        public Int64 Garbage { get; set; }
        public Int64 SizeGarbage { get; set; }
    }
    // CountObjectsOptions contains optional arguments for counting objects.
    // Docs: https://git-scm.com/docs/git-count-objects
    public class CountObjectsOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // FsckOptions contains optional arguments for verifying the objects.
    // Docs: https://git-scm.com/docs/git-fsck
    public class FsckOptions
    {
        // The additional arguments to be applied.
        public List<string> Args { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }

        public FsckOptions()
        {
            this.Args = new List<string>();
        }

    }
    // CatFileTypeOptions contains optional arguments for showing the object type.
    // Docs: https://git-scm.com/docs/git-cat-file#Documentation/git-cat-file.txt--t
    public class CatFileTypeOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // LogOptions contains optional arguments for listing commits.
    // Docs: https://git-scm.com/docs/git-log
    public class LogOptions
    {
        // The maximum number of commits to output.
        public int MaxCount { get; set; }
        // The number commits skipped before starting to show the commit output.
        public int Skip { get; set; }
        // To only show commits since the time.
        public DateTime Since { get; set; }
        // The regular expression to filter commits by their messages.
        public string GrepPattern { get; set; }
        // Indicates whether to ignore letter case when match the regular expression.
        public bool RegexpIgnoreCase { get; set; }
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CommitByRevisionOptions contains optional arguments for getting a commit.
    // Docs: https://git-scm.com/docs/git-log
    public class CommitByRevisionOptions
    {
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CommitsByPageOptions contains optional arguments for getting paginated commits.
    // Docs: https://git-scm.com/docs/git-log
    public class CommitsByPageOptions
    {
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // SearchCommitsOptions contains optional arguments for searching commits.
    // Docs: https://git-scm.com/docs/git-log
    public class SearchCommitsOptions
    {
        // The maximum number of commits to output.
        public int MaxCount { get; set; }
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // CommitsSinceOptions contains optional arguments for listing commits since a time.
    // Docs: https://git-scm.com/docs/git-log
    public class CommitsSinceOptions
    {
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // DiffNameOnlyOptions contains optional arguments for listing changed files.
    // Docs: https://git-scm.com/docs/git-diff#Documentation/git-diff.txt---name-only
    public class DiffNameOnlyOptions
    {
        // Indicates whether two commits should have a merge base.
        public bool NeedsMergeBase { get; set; }
        // The relative path of the repository.
        public string Path { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }

    // LatestCommitTimeOptions contains optional arguments for getting the latest commit time.
    public class LatestCommitTimeOptions
    {
        // To get the latest commit time of the branch. When not set, it checks all branches.
        public string Branch { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // DiffOptions contains optional arguments for parsing diff.
    // Docs: https://git-scm.com/docs/git-diff#Documentation/git-diff.txt---full-index
    public class DiffOptions
    {
        // The commit ID to used for computing diff between a range of commits (base, revision]. When not set,
        // only computes diff for a single commit at revision.
        public string Base { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // RawDiffFormat is the format of a raw diff.
    public enum RawDiffFormat
    {
        RawDiffNormal,
        RawDiffPatch
    }

    public class RawDiffFormatOpt
    {
        public static RawDiffFormat? ToRawDiffFormat(string msg)
        {
            switch (msg)
            {
                case "diff":
                    return RawDiffFormat.RawDiffNormal;
                case "patch":
                    return RawDiffFormat.RawDiffPatch;
            }
            return null;
        }
    }

    public static class RawDiffFormatExtensions
    {
        public static string ToTypeString(this RawDiffFormat ot)
        {
            switch (ot)
            {
                case RawDiffFormat.RawDiffNormal:
                    return "diff";
                case RawDiffFormat.RawDiffPatch:
                    return "patch";
                default:
                    return "";
            }
        }
    }

    // RawDiffOptions contains optional arguments for dumpping a raw diff.
    // Docs: https://git-scm.com/docs/git-format-patch
    public class RawDiffOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }

    // DiffBinaryOptions contains optional arguments for producing binary patch.
    public class DiffBinaryOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }

    // MergeBaseOptions contains optional arguments for getting merge base.
    // // Docs: https://git-scm.com/docs/git-merge-base
    public class MergeBaseOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // Reference contains information of a Git reference.
    public class Reference
    {
        public string ID { get; set; }
        public string Refspec { get; set; }
    }
    // ShowRefVerifyOptions contains optional arguments for verifying a reference.
    // Docs: https://git-scm.com/docs/git-show-ref#Documentation/git-show-ref.txt---verify
    public class ShowRefVerifyOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // SymbolicRefOptions contains optional arguments for get and set symbolic ref.
    public class SymbolicRefOptions
    {
        // The name of the symbolic ref. When not set, default ref "HEAD" is used.
        public string Name { get; set; }
        // The name of the reference, e.g. "refs/heads/master". When set, it will
        // be used to update the symbolic ref.
        public string Ref { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // ShowRefOptions contains optional arguments for listing references.
    // Docs: https://git-scm.com/docs/git-show-ref
    public class ShowRefOptions
    {
        public ShowRefOptions()
        {
            this.Patterns = new List<string>();
        }
        // Indicates whether to include heads.
        public bool Heads { get; set; }
        // Indicates whether to include tags.
        public bool Tags { get; set; }
        // The list of patterns to filter results.
        public List<string> Patterns { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // DeleteBranchOptions contains optional arguments for deleting a branch.
    // // Docs: https://git-scm.com/docs/git-branch
    public class DeleteBranchOptions
    {
        // Indicates whether to force delete the branch.
        public bool Force { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // LsRemoteOptions contains arguments for listing references in a remote repository.
    // Docs: https://git-scm.com/docs/git-ls-remote
    public class LsRemoteOptions
    {
        // Indicates whether to include heads.
        public bool Heads { get; set; }
        // Indicates whether to include tags.
        public bool Tags { get; set; }
        // Indicates whether to not show peeled tags or pseudorefs.
        public bool Refs { get; set; }
        // The list of patterns to filter results.
        public List<string> Patterns { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
    // AddRemoteOptions contains options to add a remote address.
    // Docs: https://git-scm.com/docs/git-remote#Documentation/git-remote.txt-emaddem
    public class AddRemoteOptions
    {
        // Indicates whether to execute git fetch after the remote information is set up.
        public bool Fetch { get; set; }
        // Indicates whether to add remote as mirror with --mirror=fetch.
        public bool MirrorFetch { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }

    }
    // RemoveRemoteOptions contains arguments for removing a remote from the repository.
    // Docs: https://git-scm.com/docs/git-remote#Documentation/git-remote.txt-emremoveem
    public class RemoveRemoteOptions
    {
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }
    }
}