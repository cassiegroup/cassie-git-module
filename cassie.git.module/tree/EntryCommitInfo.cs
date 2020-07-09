using System;
using cassie.git.module.commits;

namespace cassie.git.module.tree
{
    // EntryCommitInfo contains a tree entry with its commit information.
    public  class EntryCommitInfo
    {
        public TreeEntry Entry { get; set; }
        public Commit Commit { get; set; }
        public Submodule Submodule { get; set; }
    }
    // CommitsInfoOptions contains optional arguments for getting commits information.
    public class CommitsInfoOptions
    {
        // The relative path of the repository.
        public string Path { get; set; }
        // The maximum number of goroutines to be used for getting commits information.
        // When not set (i.e. <=0), runtime.GOMAXPROCS is used to determine the value.
        public int MaxConcurrency { get; set; }
        // The timeout duration before giving up for each shell command execution.
        // The default timeout duration will be used when not supplied.
        public Int64 Timeout { get; set; }

    }
}