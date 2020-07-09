using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cassie.git.module.tree;

namespace cassie.git.module
{
    public class Blob : TreeEntry
    {
        // public TreeEntry TreeEntry{get;set;}

        // public Blob(TreeEntry te)
        // {
        //     this.TreeEntry = te;
        // }
        public async Task<Result> Get()
        {
            var cmd = new Command("show",this.ID.String());
            var result = await cmd.RunAsync(dir:this.Parent.Repo.Path,splitChar:'\n');
            if(!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            return result;
        }
        public byte[] Bytes()
        {
            
            var cmd = new Command("show",this.ID.String());
            var result = cmd.RunStream(dir: this.Parent.Repo.Path);
            return result;
        }
        //Func<byte[],int> return a
        //if a > 0, read 
        //if a <= 0, jump read loop
        public byte[] Pipeline(Func<byte[], int> stdOut)
        {
            var cmd = new Command("show",this.ID.String());
            var ms = cmd.RunStream(stdOut,dir:this.Parent.Repo.Path,blockSize:128);
            return ms;
        }

    }
}