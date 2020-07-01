using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module
{
    public class Tag
    {
        public SHA1 ID { get; set; }
        public Signature Author { get; set; }
        public Signature Committer { get; set; }
        public string Message { get; set; }
        public List<SHA1> Parents { get; set; }
        public Tree Tree { get; set; }
        public ConcurrentDictionary<string,object> Submodules { get; set; }

        public Tag()
        {
            this.Parents = new List<SHA1>();
            this.Submodules = new ConcurrentDictionary<string, object>();
            this.Tree = new Tree();
        }

        
    }
}