using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.commits;
using cassie.git.module.repo;
using Xunit;

namespace cassie.git.module.test
{
    public class CommitTests
    {
        private string repoPath = "/Users/lucas/Documents/GitHub/cassie-git-module/testdata/testrepo.git";
        [Fact]
        public async void Commit_Parent()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("435ffceb7ba576c937e922766e37d4f7abdcc122");
            Assert.Equal(2, c.ParentsCount());

            var test0 = await c.GetParent(0);
            Assert.Equal("a13dba1e469944772490909daa58c53ac8fa4b0d", test0.ID.String());

            var test1 = await c.GetParent(1);
            Assert.Equal("7c5ee6478d137417ae602140c615e33aed91887c", test1.ID.String());
        }

        [Fact]
        public async void Commit_CommitByPath()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("2a52e96389d02209b451ae1ddf45d645b42d744c");
            var cc = await c.CommitByPath(new CommitByRevisionOptions { Path = "", Timeout = 60 });
            Assert.Equal("2a52e96389d02209b451ae1ddf45d645b42d744c", cc.ID.String());
            cc = await c.CommitByPath(new CommitByRevisionOptions { Path = "resources/labels.properties", Timeout = 60 });
            Assert.Equal("755fd577edcfd9209d0ac072eed3b022cbe4d39b", cc.ID.String());
        }
        [Fact]
        public async void Commit_CommitByPage()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("f5ed01959cffa4758ca0a49bf4c34b138d7eab0a");
            var cc = await c.CommitsByPage(0, 2, new CommitsByPageOptions { Timeout = 60 });

            Assert.Equal("f5ed01959cffa4758ca0a49bf4c34b138d7eab0a", cc.First().ID.String());
            Assert.Equal("9cdb160ee4118035bf73c744e3bf72a1ba16484a", cc.Last().ID.String());

            cc = await c.CommitsByPage(1, 2, new CommitsByPageOptions { Timeout = 60 });

            Assert.Equal("f5ed01959cffa4758ca0a49bf4c34b138d7eab0a", cc.First().ID.String());
            Assert.Equal("9cdb160ee4118035bf73c744e3bf72a1ba16484a", cc.Last().ID.String());

            cc = await c.CommitsByPage(2, 2, new CommitsByPageOptions { Timeout = 60 });

            Assert.Equal("dc64fe4ab8618a5be491a9fca46f1585585ea44e", cc.First().ID.String());
            Assert.Equal("32c273781bab599b955ce7c59d92c39bedf35db0", cc.Last().ID.String());

            cc = await c.CommitsByPage(3, 2);

            Assert.Equal("755fd577edcfd9209d0ac072eed3b022cbe4d39b", cc.First().ID.String());

            cc = await c.CommitsByPage(4, 2);

            Assert.Empty(cc);

            cc = await c.CommitsByPage(2, 2, new CommitsByPageOptions { Path = "src" });

            Assert.Equal("755fd577edcfd9209d0ac072eed3b022cbe4d39b", cc.First().ID.String());

        }


        [Fact]
        public async void Commit_SearchCommits()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("2a52e96389d02209b451ae1ddf45d645b42d744c");
            var cc = await c.SearchCommits("");

            Assert.Equal("2a52e96389d02209b451ae1ddf45d645b42d744c", cc.ElementAt(0).ID.String());
            Assert.Equal("57d0bf61e57cdacb309ebd1075257c6bd7e1da81", cc.ElementAt(1).ID.String());
            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(2).ID.String());
            Assert.Equal("818f033c4ae7f26b2b29e904942fa79a5ccaadd0", cc.ElementAt(3).ID.String());
            Assert.Equal("369adba006a1bbf25e957a8622d2b919c994d035", cc.ElementAt(4).ID.String());
            Assert.Equal("2956e1d20897bf6ed509f6429d7f64bc4823fe33", cc.ElementAt(5).ID.String());
            Assert.Equal("333fd9bc94084c3e07e092e2bc9c22bab4476439", cc.ElementAt(6).ID.String());
            Assert.Equal("f5ed01959cffa4758ca0a49bf4c34b138d7eab0a", cc.ElementAt(7).ID.String());
            Assert.Equal("9cdb160ee4118035bf73c744e3bf72a1ba16484a", cc.ElementAt(8).ID.String());
            Assert.Equal("dc64fe4ab8618a5be491a9fca46f1585585ea44e", cc.ElementAt(9).ID.String());
            Assert.Equal("32c273781bab599b955ce7c59d92c39bedf35db0", cc.ElementAt(10).ID.String());
            Assert.Equal("755fd577edcfd9209d0ac072eed3b022cbe4d39b", cc.ElementAt(11).ID.String());

            cc = await c.SearchCommits("", new SearchCommitsOptions { MaxCount = 3 });

            Assert.Equal(3, cc.Count);
            Assert.Equal("2a52e96389d02209b451ae1ddf45d645b42d744c", cc.ElementAt(0).ID.String());
            Assert.Equal("57d0bf61e57cdacb309ebd1075257c6bd7e1da81", cc.ElementAt(1).ID.String());
            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(2).ID.String());


            cc = await c.SearchCommits("feature");

            Assert.Equal(2, cc.Count);

            Assert.Equal("2a52e96389d02209b451ae1ddf45d645b42d744c", cc.ElementAt(0).ID.String());
            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(1).ID.String());


            cc = await c.SearchCommits("add.*", new SearchCommitsOptions { Path = "src" });
            Assert.Equal(5, cc.Count);

            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(0).ID.String());
            Assert.Equal("818f033c4ae7f26b2b29e904942fa79a5ccaadd0", cc.ElementAt(1).ID.String());
            Assert.Equal("333fd9bc94084c3e07e092e2bc9c22bab4476439", cc.ElementAt(2).ID.String());
            Assert.Equal("32c273781bab599b955ce7c59d92c39bedf35db0", cc.ElementAt(3).ID.String());
            Assert.Equal("755fd577edcfd9209d0ac072eed3b022cbe4d39b", cc.ElementAt(4).ID.String());


            cc = await c.SearchCommits("add.*", new SearchCommitsOptions { MaxCount = 2, Path = "src" });
            Assert.Equal(2, cc.Count);

            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(0).ID.String());
            Assert.Equal("818f033c4ae7f26b2b29e904942fa79a5ccaadd0", cc.ElementAt(1).ID.String());

        }

        [Fact]
        public async void Commit_ShowNameStatus()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("755fd577edcfd9209d0ac072eed3b022cbe4d39b");
            var cc = await c.ShowNameStatus();
            Assert.Equal(3, cc.Added.Count);

            Assert.Equal("README.txt", cc.Added.ElementAt(0));
            Assert.Equal("resources/labels.properties", cc.Added.ElementAt(1));
            Assert.Equal("src/Main.groovy", cc.Added.ElementAt(2));

            c = await repo.CatFileCommit("32c273781bab599b955ce7c59d92c39bedf35db0");
            cc = await c.ShowNameStatus();
            Assert.Equal(1, cc.Modified.Count);
            Assert.Equal("src/Main.groovy", cc.Modified.ElementAt(0));

            c = await repo.CatFileCommit("dc64fe4ab8618a5be491a9fca46f1585585ea44e");
            cc = await c.ShowNameStatus();
            Assert.Equal(1, cc.Added.Count);
            Assert.Equal(1, cc.Modified.Count);
            Assert.Equal("src/Square.groovy", cc.Added.ElementAt(0));
            Assert.Equal("src/Main.groovy", cc.Modified.ElementAt(0));

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.ShowNameStatus();
            Assert.Equal(1, cc.Removed.Count);
            Assert.Equal("fix.txt", cc.Removed.ElementAt(0));

        }

        [Fact]
        public async void Commit_CommitCount()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("755fd577edcfd9209d0ac072eed3b022cbe4d39b");
            var cc = await c.CommitsCount("");
            Assert.Equal(1, cc);

            

            c = await repo.CatFileCommit("f5ed01959cffa4758ca0a49bf4c34b138d7eab0a");
            cc = await c.CommitsCount("");
            Assert.Equal(5, cc);

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.CommitsCount("");
            Assert.Equal(27, cc);

            c = await repo.CatFileCommit("7c5ee6478d137417ae602140c615e33aed91887c");
            cc = await c.CommitsCount("",new RevListCountOptions{Path="README.txt"});
            Assert.Equal(3, cc);

            c = await repo.CatFileCommit("7c5ee6478d137417ae602140c615e33aed91887c");
            cc = await c.CommitsCount("", new RevListCountOptions { Path = "resources" });
            Assert.Equal(1, cc);

        }
        [Fact]
        public async void Commit_FilesChangedAfter()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            var cc = await c.FilesChangedAfter("ef7bebf8bdb1919d947afe46ab4b2fb4278039b3");
            Assert.Equal(1, cc.Count);
            Assert.Equal("fix.txt",cc.ElementAt(0));

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.FilesChangedAfter("45a30ea9afa413e226ca8614179c011d545ca883");
            Assert.Equal(3, cc.Count);
            Assert.Equal("fix.txt", cc.ElementAt(0));
            Assert.Equal("pom.xml", cc.ElementAt(1));
            Assert.Equal("src/test/java/com/github/AppTest.java", cc.ElementAt(2));

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.FilesChangedAfter("45a30ea9afa413e226ca8614179c011d545ca883",new DiffNameOnlyOptions{Path="src"});
            Assert.Equal(1, cc.Count);
            Assert.Equal("src/test/java/com/github/AppTest.java", cc.ElementAt(0));

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.FilesChangedAfter("45a30ea9afa413e226ca8614179c011d545ca883", new DiffNameOnlyOptions { Path = "resources" });
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public async void Commit_CommitsAfter()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            var cc = await c.CommitsAfter("45a30ea9afa413e226ca8614179c011d545ca883");
            Assert.Equal(3, cc.Count);
            Assert.Equal("978fb7f6388b49b532fbef8b856681cfa6fcaa0a", cc.ElementAt(0).ID.String());
            Assert.Equal("ef7bebf8bdb1919d947afe46ab4b2fb4278039b3", cc.ElementAt(1).ID.String());
            Assert.Equal("ebbbf773431ba07510251bb03f9525c7bab2b13a", cc.ElementAt(2).ID.String());

            c = await repo.CatFileCommit("978fb7f6388b49b532fbef8b856681cfa6fcaa0a");
            cc = await c.CommitsAfter("45a30ea9afa413e226ca8614179c011d545ca883", new RevListOptions{Path="src"});
            Assert.Equal(1, cc.Count);
            Assert.Equal("ebbbf773431ba07510251bb03f9525c7bab2b13a", cc.ElementAt(0).ID.String());
        }

        [Fact]
        public async void Commit_Ancestors()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("2a52e96389d02209b451ae1ddf45d645b42d744c");
            var cc = await c.Ancestors(new LogOptions{MaxCount=3});
            Assert.Equal(3, cc.Count);
            Assert.Equal("57d0bf61e57cdacb309ebd1075257c6bd7e1da81", cc.ElementAt(0).ID.String());
            Assert.Equal("cb2d322bee073327e058143329d200024bd6b4c6", cc.ElementAt(1).ID.String());
            Assert.Equal("818f033c4ae7f26b2b29e904942fa79a5ccaadd0", cc.ElementAt(2).ID.String());

            c = await repo.CatFileCommit("755fd577edcfd9209d0ac072eed3b022cbe4d39b");
            cc = await c.Ancestors();
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public async void Commit_IsImageFile()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("4e59b72440188e7c2578299fc28ea425fbe9aece");
            var cc = await c.IsImageFile("gogs/docs-api");
            Assert.False(cc);

            c = await repo.CatFileCommit("4e59b72440188e7c2578299fc28ea425fbe9aece");
            cc = await c.IsImageFileByIndex("adfd6da3c0a3fb038393144becbf37f14f780087");
            Assert.False(cc);

            c = await repo.CatFileCommit("4e59b72440188e7c2578299fc28ea425fbe9aece");
            cc = await c.IsImageFileByIndex("2ce918888b0fdd4736767360fc5e3e83daf47fce");
            Assert.True(cc);
           
        }
        [Fact]
        public async void Commit_CreateArchive()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("755fd577edcfd9209d0ac072eed3b022cbe4d39b");
            var tempPath = AppDomain.CurrentDomain.BaseDirectory+DateTime.Now.ToString("yyyyMMddHHmmss")+"."+ArchiveFormat.ArchiveZip.ToTypeString();
            await c.CreateArchive(ArchiveFormat.ArchiveZip,tempPath);

            Assert.True(System.IO.File.Exists(tempPath));
            System.IO.File.Delete(tempPath);

        }
        [Fact]
        public async void Commit_GetSubmodules()
        {
            var repo = new Repository(repoPath);
            var c = await repo.CatFileCommit("4e59b72440188e7c2578299fc28ea425fbe9aece");
            var mod = await c.GetSubmodule("gogs/docs-api");
            Assert.Equal("gogs/docs-api",mod.Name);
            Assert.Equal("https://github.com/gogs/docs-api.git",mod.URL);
            Assert.Equal("6b08f76a5313fa3d26859515b30aa17a5faa2807", mod.Commit);
        }
    }
}