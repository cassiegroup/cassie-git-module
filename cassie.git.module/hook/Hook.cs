using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cassie.git.module.repo;

namespace cassie.git.module.hook
{
    public class Hook
    {
        // Hook contains information of a Git hook.
        public HookName Name { get; set; }
        // The absolute file path of the hook.
        public string Path { get; set; }
        // Indicates whether this hook is read from the sample.
        public bool IsSample { get; set; }
        // The content of the hook.
        public string Content { get; set; }
        // Update writes the content of the Git hook on filesystem. It updates the memory copy of
        // the content as well.

		public Hook(){}
		public Hook(string dir,HookName name):this()
		{
			this.Path = dir;
			this.Name = name;
		}
        public void Update(string content)
		{
			this.Content = content.Trim();
			this.Content = this.Content.Replace(@"\r","");
			if(!string.IsNullOrEmpty(this.Path))
			{
                try
                {
                    Utils.Mkfile(this.Path);
                    File.WriteAllText(this.Path,content);
                }
                catch (System.Exception)
                {
                    throw;
                }  
			} 
			this.IsSample = false;

		}

	}

}