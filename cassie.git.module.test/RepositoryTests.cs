using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.hook;
using cassie.git.module.repo;
using Xunit;

namespace cassie.git.module.test
{
    public class RepositoryTests
    {

        [Fact]
        public async void Repository_RepoShowNameStatus()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");

            var result = await repo.ShowNameStatus("master", new ShowNameStatusOptions { Timeout = 60 });
            ;

        }

        [Fact]
        public async void Repository_CountObjects()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");

            await repo.CountObjects(new CountObjectsOptions { Timeout = 60 });
            ;

        }

        [Fact]
        public async void Repository_DiffNameOnly()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var baseBranch = "c18751fc502e9d808beaadb879341a56144c566f";
            var head = "516c7ab50500cff02f7b62165a6fe43daf688e6b";
            var result = await repo.DiffNameOnly(baseBranch, head, new DiffNameOnlyOptions { Timeout = 60 });
            ;

        }

        [Fact]
        public async void Repository_CatFileCommit()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var rev = "516c7ab50500cff02f7b62165a6fe43daf688e6b";
            var result = await repo.CatFileCommit(rev, new CatFileCommitOptions { Timeout = 60 });
            ;

        }
        [Fact]
        public async void Repository_LatestCommitTime()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var result = await repo.LatestCommitTime(new LatestCommitTimeOptions { Timeout = 60 });
            ;

        }

        [Fact]
        public async void Repository_Diff()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module");
            var result = await repo.Diff("75bfc591c9821a7bc57183fb85e3b8272ab961fe",0,0,0,new DiffOptions{Timeout=60});
            ;

        }
        [Fact]
        public async void Repository_RawDiff()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module");
            var summary = "";
            Action<string> received = str => {
                summary += str;
            };
            await repo.RawDiff("75bfc591c9821a7bc57183fb85e3b8272ab961fe",RawDiffFormat.RawDiffNormal,received,new RawDiffOptions{Timeout=60} );
            ;

            Console.WriteLine(summary);

        }
        [Fact]
        public void Repository_Hooks()
        {
            // Save "post-receive" hook with some content
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module/testdata/testrepo.git");
            var postReceiveHook = repo.NewHook(HookConst.DefaultHooksDir,HookName.HookPostReceive);
            postReceiveHook.Update("echo $1 $2 $3 $4");
            var hooks = repo.Hooks("");
            foreach (var item in hooks)
            {
                Console.WriteLine(item.Content);
                Assert.NotEmpty(item.Content);
            }

        }
        [Fact]
        public async void Repository_Tags()
        {
            // Save "post-receive" hook with some content
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module/testdata/testrepo.git");
            var result = await repo.Tags(new TagsOptions{Timeout=60});
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void Repository_CreateTag()
        {
            // Save "post-receive" hook with some content
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module");
            await repo.CreateTag("v2.0.0", "master", new CreateTagOptions{Timeout=60});
            var result = await repo.HasReference(RepositoryConst.RefsTags + "v2.0.0");
            Assert.True(result);
        }

        [Fact]
        public async void Repository_DeleteTag()
        {
            // Save "post-receive" hook with some content
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module");
            Assert.True(await repo.HasReference(RepositoryConst.RefsTags + "v2.0.0"));
            await repo.DeleteTag("v2.0.0",  new DeleteTagOptions { Timeout = 60 });
           
        }
        [Fact]
        public async void Repository_LsTree()
        {
            // Save "post-receive" hook with some content
            var repo = new Repository("/Users/lucas/Documents/GitHub/cassie-git-module");
            var result = await repo.LsTree("190ba356f97694c544d53dc19e159a9717e43740",new LsTreeOptions{Timeout=60});
            Assert.NotNull(result);

        }
    }
}