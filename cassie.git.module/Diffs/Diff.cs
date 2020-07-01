using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module.diffs
{
    // Diff represents a Git diff.
    public class Diff
    {
        // The files in the diff
        public List<DiffFile> Files { get; set; }
        internal int totalAdditions { get; set; }
        internal int totalDeletions { get; set; }
        internal bool isIncomplete { get; set; }
        public Diff()
        {
            this.Files = new List<DiffFile>();
        }
        // NumFiles returns the number of files in the diff.
        public int NumFiles()
        {
            return this.Files.Count();
        }
        // TotalAdditions returns the total additions in the diff.
        public int TotalAdditions()
        {
            return this.totalAdditions;
        }
        // TotalDeletions returns the total deletions in the diff.
        public int TotalDeletions()
        {
            return this.totalDeletions;
        }
        // IsIncomplete returns true if the file is incomplete to the entire diff.
        public bool IsIncomplete()
        {
            return this.isIncomplete;
        }
    }
    
}