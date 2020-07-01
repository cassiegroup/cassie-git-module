using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module.diffs
{
    // DiffLine represents a line in diff.
    public class DiffLine
    {
        // The type of the line
        public DiffLineType Type { get; set; }
        // The content of the line
        public string Content { get; set; }
        // The left line number
        public int LeftLine { get; set; }
        // The right line number
        public int RightLine { get; set; }
    }
}