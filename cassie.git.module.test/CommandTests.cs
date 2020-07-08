using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace cassie.git.module.test
{
    public class CommandTests
    {
        [Fact]
        public void Commnad_True_String()
        {
            var commnd = new Command();
            Assert.True(commnd.String()=="git");
            commnd = new Command("version");
            Assert.True(commnd.String() == "git version");
            commnd = new Command(new string[]{ "config", "--global", "http.proxy", "http://localhost:8080" });
            Assert.True(commnd.String() == "git config --global http.proxy http://localhost:8080");
        }

        [Fact]
        public void Commnad_True_AddArgs()
        {
            var commnd = new Command();
            commnd.AddArgs("push");
            commnd.AddArgs("origin","master");
            Assert.True(Enumerable.SequenceEqual(new string[]{"push", "origin", "master" },commnd.GetArgs()));
        }

        [Fact]
        public void Commnad_True_AddEnvs()
        {
            var commnd = new Command();
            commnd.AddEnvs( "GIT_DIR","/tmp");
            var dic = new Dictionary<string,string>();
            dic.Add("HOME", "/Users/unknwon");
            dic.Add("GIT_EDITOR", "code");
            commnd.AddEnvs(dic);    
            
            var dicResult = new Dictionary<string,string>();
            dicResult.Add("GIT_DIR", "/tmp");
            dicResult.Add("HOME", "/Users/unknwon");
            dicResult.Add("GIT_EDITOR", "code");
            Assert.True(dicResult.OrderBy(x => x.Key).SequenceEqual(commnd.GetEnvs().OrderBy(x => x.Key)));
        }

        [Fact]
        public async void Commnad_True_RunAsync()
        {
            var commnd = new Command("version");
            var result = await commnd.RunAsync(timeoutMs:100);
            Console.WriteLine(result.StdOut);
            
        }
    }
}