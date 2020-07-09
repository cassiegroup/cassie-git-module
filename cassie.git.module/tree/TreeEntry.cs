using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module.tree
{
    public class TreeEntry : EntryBase, IComparable<TreeEntry>
    {
        public EntryMode Mode { get; set; }
        public ObjectType Typ { get; set; }
        public SHA1 ID { get; set; }
        public string Name { get; set; }
        public Tree Parent { get; set; }

        private Int64 size;


        public bool IsTree()
        {
            return this.Mode == EntryMode.EntryTree;
        }

        public bool IsBlob()
        {
            return this.Mode == EntryMode.EntryBlob;
        }

        public bool IsExec()
        {
            return this.Mode == EntryMode.EntryExec;
            
        }

        public bool IsSymlink()
        {
            return this.Mode == EntryMode.EntrySymlink;
        }

        public bool IsCommit()
        {
            return this.Mode == EntryMode.EntryCommit;
        }

        public async Task<Int64> GetSize()
        {
            if(IsTree()) return -1;
            var cmd = new Command("cat-file", "-s", this.ID.String());
            var result = await cmd.RunAsync(dir:this.Parent.Repo.Path);
            var size = Convert.ToInt64(result.StdOut.Trim());
            return size;
        }

        public Blob NewBlob()
        {
            var b = new Blob();
            Utils.CopyAllTo(this,b);
            return b;
        }

        public int CompareTo(TreeEntry other)
        {
            if((this.IsTree() || this.IsCommit()) && !other.IsTree() && !other.IsCommit())
            {
                return 1;
            }
            // if ((other.IsTree() || other.IsCommit()) && !this.IsTree() && !this.IsCommit())
            // {
            //     return 1;
            // }
            
            if(string.Compare(this.Name,other.Name)>0) return 1;
            else return -1;
        }   
    }

    // There are only a few file modes in Git. They look like unix file modes, but they can only be
    // one of these.
    public enum EntryMode
    {
        EntryTree = 0040000,
        EntryBlob = 0100644,
        EntryExec = 0100755,
        EntrySymlink = 0120000,
        EntryCommit = 0160000
    }
}