using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cassie.git.module.repo;

namespace cassie.git.module
{
    public class Tree
    {
        public SHA1 ID { get; set; }
        public Tree Parent { get; set; }
        public Repository Repo { get; set; }

        public List<TreeEntry> Entries { get; set; }

        // public TreeEntry NewTreeEntry(string subPath, params Int64[] opts)
        // {
            
        // }
        
        public Tree Subtree(string subPath,params Int64[] opts)
        {
            if(string.IsNullOrEmpty(subPath)) return this;
            var paths = subPath.Split('/');

            foreach (var item in paths)
            {
                
            }
            return null;
        }
    }
}