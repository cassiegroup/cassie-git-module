using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cassie.git.module.repo;

namespace cassie.git.module
{
    // Submodule contains information of a Git submodule.
    public class Commit
    {
        // The SHA-1 hash of the commit.
        public SHA1 ID { get; set; }
        // Author *Signature
        public Signature Author { get; set; }
        // The committer of the commit.
        public Signature Committer { get; set; }
        // The full commit message.
        public string Message { get; set; }

        public Tree Tree { get; set; }

        public List<SHA1> Parents { get; set; }
        private ConcurrentDictionary<string,object> submodules { get; set; }

        public Commit()
        {
            this.Parents = new List<SHA1>();
            this.submodules = new ConcurrentDictionary<string, object>();
        }

        // Summary returns first line of commit message.
        public string Summary()
        {
            return this.Message.Split('\n').First();
        }

        // ParentsCount returns number of parents of the commit.
        // It returns 0 if this is the root commit, otherwise returns 1, 2, etc.
        public int ParentsCount()
        {
            return this.Parents.Count;
        }

        // ParentID returns the SHA-1 hash of the n-th parent (0-based) of this commit.
        // It returns an ErrParentNotExist if no such parent exists.
        public SHA1 ParentID(int n)
        {
            if (n >= this.Parents.Count)
            {
                throw new Exception("n can not be greater than parents count");
            }

            return this.Parents[n];
        }

        // Parent returns the n-th parent commit (0-based) of this commit.
        // It returns ErrRevisionNotExist if no such parent exists.
        public async Task<Commit> Parent(int n, params CatFileCommitOptions[] opts)
        {
            var id = ParentID(n);
            return await this.Tree.Repo.CatFileCommit(id.ToString(),opts);
        }

    }

}