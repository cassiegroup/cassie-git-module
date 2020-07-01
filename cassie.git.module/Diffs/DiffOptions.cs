using System;
using System.Collections.Generic;

namespace cassie.git.module.diffs
{
    // DiffLineType is the line type in diff.
    public enum DiffLineType
    {
        DiffLinePlain = 1,
        DiffLineAdd,
        DiffLineDelete,
        DiffLineSection
    }
    // DiffFileType is the file status in diff.
    public enum DiffFileType
    {
        DiffFileAdd = 1,
        DiffFileChange,
        DiffFileDelete,
        DiffFileRename
    }

}