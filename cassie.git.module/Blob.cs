using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module
{
    public class Blob
    {
        public TreeEntry TreeEntry{get;set;}

        public Blob(TreeEntry te)
        {
            this.TreeEntry = te;
        }
        public async Task<Result> Bytes(Action<string> stdOut = null,
                                           Action<string> stdErr = null)
        {
            if(this.TreeEntry == null) return null;
            var cmd = new Command("show",TreeEntry.ID.ToString());
            var result = await cmd.RunAsync(dir:TreeEntry.Parent.Repo.Path,stdOut:stdOut,stdErr:stdErr);
            return result;
        }

        public async Task<Result> Pipeline()
        {
            var cmd = new Command("show",this.TreeEntry.ID.String());
            return await cmd.RunAsync(dir:this.TreeEntry.Parent.Repo.Path);
        }

    }
}