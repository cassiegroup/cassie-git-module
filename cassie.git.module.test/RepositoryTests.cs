using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async void RepositoryCommit_DiffNameOnly()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var baseBranch = "c18751fc502e9d808beaadb879341a56144c566f";
            var head = "516c7ab50500cff02f7b62165a6fe43daf688e6b";
            var result = await repo.DiffNameOnly(baseBranch, head, new DiffNameOnlyOptions { Timeout = 60 });
            ;

        }

        [Fact]
        public async void RepositoryCommit_CatFileCommit()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var rev = "516c7ab50500cff02f7b62165a6fe43daf688e6b";
            var result = await repo.CatFileCommit(rev, new CatFileCommitOptions { Timeout = 60 });
            ;

        }
        [Fact]
        public async void RepositoryCommit_LatestCommitTime()
        {
            var repo = new Repository("/Users/lucas/Documents/GitHub/gogs");
            var result = await repo.LatestCommitTime(new LatestCommitTimeOptions { Timeout = 60 });
            ;

        }
        
    }
}