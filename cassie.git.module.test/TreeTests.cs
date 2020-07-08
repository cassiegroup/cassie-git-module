using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.repo;
using Xunit;

namespace cassie.git.module.test
{
    public class TreeTests
    {
        private string repoPath = "/Users/lucas/Documents/GitHub/cassie-git-module/testdata/testrepo.git";
        
        [Fact]
        public async void Tree_NewTreeEntry()
        {
            var repo = new Repository(repoPath);
            var tree = await repo.LsTree("master");
            var e = await tree.NewTreeEntry("");
            Assert.Equal(tree.TreeID.String(),e.ID.String());
            Assert.Equal(ObjectType.ObjectTree, e.Typ);
            Assert.True(e.IsTree());
        }

        [Fact]
        public async void Tree_NewBlob()
        {
            var repo = new Repository(repoPath);
            var tree = await repo.LsTree("d58e3ef9f123eea6857161c79275ee22b228f659");
            var result = await tree.NewBlob("README.txt");
            Assert.True(result.IsBlob());

            result = await tree.NewBlob("run.sh");
            Assert.True(result.IsExec());
        }
    }
}