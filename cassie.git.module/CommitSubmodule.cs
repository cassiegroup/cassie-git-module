using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module
{
    // Submodule contains information of a Git submodule.
    public class CommitSubmodule
    {
        // The name of the submodule.
        public string Name { get; set; }
        // The URL of the submodule.
        public string URL { get; set; }
        // The commit ID of the subproject.
        public string Commit { get; set; }
    }

    public class CommitSubmoduleOpr
    {
        // Submodules contains information of submodules.
        public ConcurrentDictionary<string,object> ObjectCache = new ConcurrentDictionary<string, object>();

        // public ConcurrentDictionary<string, object> Submodules()
        // {

        // }

    }
}