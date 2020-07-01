using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cassie.git.module.diffs
{
    public class FileResult
    {
        public List<string> Result { get; set; }
        public int Cursor { get; set; }
    }
    public class DiffParser
    {
        protected int maxFiles { get; set; }
        protected int maxFileLines { get; set; }
        protected int maxLineChars { get; set; }
        private string diffHead = "diff --git ";

        public DiffParser(int maxFiles, int maxFileLines, int maxLineChars)
        {
            this.maxFileLines = maxFileLines;
            this.maxFiles = maxFiles;
            this.maxLineChars = maxLineChars;
        }
        private Tuple<DiffFile, int> parseFileHeader(FileResult fr)
        {
            var line = fr.Result[fr.Cursor];
            // Note: In case file name is surrounded by double quotes (it happens only in git-shell).
            // e.g. diff --git "a/xxx" "b/xxx"
            var middle = 0;
            var hasQuote = line.Last() == '"';
            if (hasQuote) middle = line.IndexOf(" \"b/");
            else middle = line.IndexOf(" b/");
            var beg = diffHead.Length;
            var a = line.Substring(beg + 2, middle - beg - 2);
            var b = line.Substring(middle + 3);
            if (hasQuote)
            {
                a = a.Substring(1);
                b = b.Substring(1);
            }
            var file = new DiffFile
            {
                Name = a,
                Type = DiffFileType.DiffFileChange
            };
            for (int i = fr.Cursor + 1; i < fr.Result.Count; i++)
            {
                fr.Cursor = i;
                line = fr.Result[i];
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("new file"))
                {
                    file.Type = DiffFileType.DiffFileAdd;
                    file.isSubmodule = line.EndsWith(" 160000");
                }
                if (line.StartsWith("deleted"))
                {
                    file.Type = DiffFileType.DiffFileDelete;
                    file.isSubmodule = line.EndsWith(" 160000");
                }
                if (line.StartsWith("index"))
                {
                    var raw = line.Substring(6);
                    var fields = Regex.Split(raw, @"\s+");
                    var shas = Regex.Split(fields[0], @"\..");
                    if (shas.Length != 2) throw new Exception("malformed index: expect two SHAs in the form of <old>..<new>");
                    if (file.IsDeleted()) file.Index = shas[0];
                    else file.Index = shas[1];
                    return new Tuple<DiffFile, int>(file, i);
                }
                if (line.StartsWith("similarity index "))
                {
                    file.Type = DiffFileType.DiffFileRename;
                    file.oldName = a;
                    file.Name = b;
                    if (line.EndsWith("100%")) return new Tuple<DiffFile, int>(file, i);
                }
                if (line.StartsWith("old mode")) return new Tuple<DiffFile, int>(file, i);


            }
            return new Tuple<DiffFile, int>(file, fr.Cursor);
        }

        public Tuple<DiffSection, bool, int> parseSection(FileResult fr)
        {
            var line = fr.Result[fr.Cursor];
            var section = new DiffSection
            {
                Lines = new List<DiffLine>
                {
                    new DiffLine
                    {
                        Type=DiffLineType.DiffLineSection,
                        Content = line
                    }
                }
            };
            // Parse line number, e.g. @@ -0,0 +1,3 @@
            int leftLine, rightLine;
            var ss = Regex.Split(line, @"\@@");
            var ranges = ss[1].Substring(1).Split(' ');
            leftLine = Convert.ToInt32(ranges[0].Split(',')[0].Substring(1));
            if (ranges.Length > 1) rightLine = Convert.ToInt32(ranges[1].Split(',')[0]);
            else rightLine = leftLine;
            for (int i = fr.Cursor + 1; i < fr.Result.Count; i++)
            {
                fr.Cursor = i;
                line = fr.Result[i];
                if (string.IsNullOrEmpty(line)) continue;
                // Make sure we're still in the section. If not, we're done with this section.
                if (line[0] != ' ' && line[0] != '+' && line[0] != '-')
                {
                    // No new line indicator
                    if (line[0] == '\\' && line.StartsWith(@"\ No newline at end of file")) continue;
                    return new Tuple<DiffSection, bool, int>(section, false, i);
                }
                // Too many characters in a single diff line
                if (this.maxLineChars > 0 && line.Length > this.maxLineChars) return new Tuple<DiffSection, bool, int>(section, true, i);
                switch (line[0])
                {
                    case ' ':
                        section.Lines.Add(new DiffLine
                        {
                            Type = DiffLineType.DiffLinePlain,
                            Content = line,
                            RightLine = rightLine,
                            LeftLine = leftLine
                        });
                        leftLine++;
                        rightLine++;
                        break;
                    case '+':
                        section.Lines.Add(new DiffLine
                        {
                            Type = DiffLineType.DiffLineAdd,
                            Content = line,
                            RightLine = rightLine
                        });
                        section.NumAdditions++;
                        rightLine++;
                        break;
                    case '-':
                        section.Lines.Add(new DiffLine
                        {
                            Type = DiffLineType.DiffLineDelete,
                            Content = line,
                            LeftLine = leftLine
                        });
                        section.NumDeletions++;
                        if(leftLine > 0) leftLine ++;
                        break;
                }
            }
            return new Tuple<DiffSection, bool, int>(section,false,fr.Cursor);
        }
        public Diff Parse(string lines, char splitChar)
        {
            var diff = new Diff();
            var file = new DiffFile();
            var currentFileLines = 0;
            if (string.IsNullOrEmpty(lines)) throw new Exception("git diff result can not be null");
            var fr = new FileResult { Result = lines.Split(splitChar).ToList(), Cursor = 0 };
            for (int i = 0; i < fr.Result.Count; i++)
            {
                fr.Cursor = i;
                var line = fr.Result[fr.Cursor];
                if (line.StartsWith("+++ ") || line.StartsWith("--- ")) continue;
                // Found new file
                if (line.StartsWith(diffHead))
                {
                    // Check if reached maximum number of files
                    if (this.maxFiles > 0 && diff.Files.Count >= this.maxFiles)
                    {
                        diff.isIncomplete = true;
                        break;
                    }
                    
                    var fh = this.parseFileHeader(fr);
                    file = fh.Item1;
                    fr.Cursor = fh.Item2;
                    currentFileLines = 0;
                    diff.Files.Add(file);
                    continue;
                }
                if(file.IsIncomplete()) continue;
                if(line.StartsWith("Binary"))
                {
                    file.isBinary = true;
                    continue;
                }
                // Loop until we found section header
                if(line[0] != '@') continue;
                // Too many diff lines for the file
                if(this.maxFileLines > 0 && currentFileLines > this.maxFileLines)
                {
                    file.isIncomplete = true;
                    diff.isIncomplete = true;
                    continue;
                }
                var fs = parseSection(fr);
                fr.Cursor = fs.Item3;
                file.Sections.Add(fs.Item1);
                file.numAdditions += fs.Item1.NumAdditions;
                file.numDeletions += fs.Item1.NumDeletions;
                diff.totalAdditions += fs.Item1.NumAdditions;
                diff.totalDeletions += fs.Item1.NumDeletions;
                currentFileLines += fs.Item1.NumLines();
                if(fs.Item2){
                    file.isIncomplete = true;
                    diff.isIncomplete = true;
                }
                
            }
            return diff;
        }

        // StreamParseDiff parses the diff read from the given io.Reader.It does parse-on-read to minimize
        // the time spent on huge diffs. It accepts a channel to notify and send error (if any) to the caller
        // when the process is done. Therefore, this method should be called in a goroutine asynchronously.
        public Diff StreamParseDiff(string lines,char splitChar)
        {
            this.maxFileLines = maxFileLines;
            this.maxFiles = maxFiles;
            this.maxLineChars = maxLineChars;
            var result = Parse(lines,splitChar);
            return result;
        }
    }


}