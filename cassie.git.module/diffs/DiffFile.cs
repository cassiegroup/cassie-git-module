using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module.diffs
{
    // DiffSection represents a section in diff.
    public class DiffFile
    {
        // The name of the file.
        public string Name { get; set; }
        // The type of the file.
        public DiffFileType Type { get; set; }
        // The index (SHA1 hash) of the file. For a changed/new file, it is the new SHA,
        // and for a deleted file it is the old SHA.
        public string Index { get; set; }
        // The sections in the file.
        public List<DiffSection> Sections { get; set; }
        internal int numAdditions { get; set; }
        internal int numDeletions { get; set; }
        internal string oldName { get; set; }
        internal bool isBinary { get; set; }
        internal bool isSubmodule { get; set; }
        internal bool isIncomplete { get; set; }

        public DiffFile()
        {
            this.Sections = new List<DiffSection>();
        }
        // NumSections returns the number of sections in the file.
        public int NumSections()
        {
            return this.Sections.Count();
        }
        // NumAdditions returns the number of additions in the file.
        public int NumAdditions()
        {
            return this.numAdditions;
        }
        // NumDeletions returns the number of deletions in the file.
        public int NumDeletions()
        {
            return this.numDeletions;
        }
        // IsCreated returns true if the file is newly created.
        public bool IsCreated()
        {
            return this.Type == DiffFileType.DiffFileAdd;
        }
        // IsDeleted returns true if the file has been deleted.
        public bool IsDeleted()
        {
            return this.Type == DiffFileType.DiffFileDelete;
        }
        // IsRenamed returns true if the file has been renamed.
        public bool IsRenamed()
        {
            return this.Type == DiffFileType.DiffFileRename;
        }
        // OldName returns previous name before renaming.
        public string OldName()
        {
            return this.oldName;
        }
        // IsBinary returns true if the file is in binary format.
        public bool IsBinary()
        {
            return this.isBinary;
        }
        // IsSubmodule returns true if the file contains information of a submodule.
        public bool IsSubmodule()
        {
            return this.isSubmodule;
        }
        // IsIncomplete returns true if the file is incomplete to the file diff.
        public bool IsIncomplete()
        {
            return this.isIncomplete;
        }
        
    }
}