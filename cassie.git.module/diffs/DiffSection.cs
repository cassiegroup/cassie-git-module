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
    public class DiffSection
    {
        // lines in the section
        public List<DiffLine> Lines { get; set; }
        internal int NumAdditions { get; set; }
        internal int NumDeletions { get; set; }

        public DiffSection()
        {
            this.Lines = new List<DiffLine>();
        }

        public int NumLines()
        {
            return this.Lines.Count();
        }
        // Line returns a specific line by given type and line number in a section.
        public DiffLine Line(DiffLineType typ, int line)
        {
            var difference = 0;
            var addCount = 0;
            var delCount = 0;
            DiffLine matchedDiffLine = null;
            foreach (var item in this.Lines)
            {
                switch (item.Type)
                {
                    case DiffLineType.DiffLineAdd:
                        addCount++;
                        break;
                    case DiffLineType.DiffLineDelete:
                        delCount++;
                        break;
                    default:
                        if (matchedDiffLine != null)
                        {
                            if (addCount == delCount) return matchedDiffLine;
                            throw new Exception("diff line error");
                        }
                        difference = item.RightLine - item.LeftLine;
                        addCount = 0;
                        delCount = 0;
                        break;

                }

                switch (typ)
                {
                    case DiffLineType.DiffLineDelete:
                        if (item.RightLine == 0 && item.LeftLine == line - difference)
                        {
                            matchedDiffLine = item;
                        }
                        break;
                    case DiffLineType.DiffLineAdd:
                        if (item.LeftLine == 0 && item.RightLine == line + difference)
                        {
                            matchedDiffLine = item;
                        }
                        break;

                }
            }
            if (addCount == delCount) {
                return matchedDiffLine;

            }
            throw new Exception("diff line error");
        }
    }
}