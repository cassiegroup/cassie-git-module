using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace cassie.git.module.test
{
    public class GitTests
    {
        private string repoPath = " ";
        private Version baseVersion = new Version("1.8.3");
        [Fact]
        public async void Git_BinVersion()
        {
            var ver = await Git.BinVersion(); 

            var result = ver.CompareTo(baseVersion);
            if(result < 0) throw new Exception("version can not be letter than 1.8.3");
            
        }

        
    }
}