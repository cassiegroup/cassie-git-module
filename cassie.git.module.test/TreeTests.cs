using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.repo;
using cassie.git.module.tree;
using Xunit;

namespace cassie.git.module.test
{
    public class TreeTests
    {
        // private string repoPath = "../../../../testdata/testrepo.git";
        
        // [Fact]
        // public async void Tree_NewTreeEntry()
        // {
        //     var repo = new Repository(repoPath);
        //     var tree = await repo.LsTree("master");
        //     var e = await tree.NewTreeEntry("");
        //     Assert.Equal(tree.TreeID.String(),e.ID.String());
        //     Assert.Equal(ObjectType.ObjectTree, e.Typ);
        //     Assert.True(e.IsTree());
        // }

        // [Fact]
        // public async void Tree_NewBlob()
        // {
        //     var repo = new Repository(repoPath);
        //     var tree = await repo.LsTree("d58e3ef9f123eea6857161c79275ee22b228f659");
        //     var result = await tree.NewBlob("README.txt");
        //     Assert.True(result.IsBlob());

        //     result = await tree.NewBlob("run.sh");
        //     Assert.True(result.IsExec());
        // }

        // [Fact]
        // public async void Tree_Sort()
        // {
        //     var repo = new Repository(repoPath);
        //     var tree = await repo.LsTree("0eedd79eba4394bbef888c804e899731644367fe");
        //     var entries = tree.Entries;
        //     //entries.Sort();
        //     var list = new List<string>();
        //     foreach (var item in entries.OrderBy(a => a.Name).OrderBy(a => a.Mode))
        //     {
        //         list.Add(item.ID.String()+"--"+item.Name+"--"+item.Mode.ToString());
        //     }

        // }

        // [Fact]
        // public async void Tree_CommitsInfo()
        // {
        //     var repo = new Repository(repoPath);
        //     var tree = await repo.LsTree("0eedd79eba4394bbef888c804e899731644367fe");
        //     var c = await repo.CatFileCommit(tree.TreeID.String());
        //     var res = await tree.CommitsInfo(c);

        //     //entries.Sort();
        //     var list = new List<string>();
        //     foreach (var item in res.OrderBy(a => a.Entry.Name))
        //     {
        //         list.Add(item.Commit.ID.String() + "--" + item.Entry.Name);
        //     }
        //     var subtree = await tree.Subtree("gogs");
        //     var es = await subtree.EntriyList();
        //     res = await subtree.CommitsInfo(c,new CommitsInfoOptions{Path="gogs"});
        // }
    }
}