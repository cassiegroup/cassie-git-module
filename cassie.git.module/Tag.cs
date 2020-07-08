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
    public class Tag
    {
        public ObjectType Typ { get; set; }
        public SHA1 ID { get; set; }
        public SHA1 CommitID { get; set; }
        public string Refspec { get; set; }
        public Signature Tagger { get; set; }
        public string Message { get; set; }

        public Repository Repo { get; set; }

        public async Task<Commit> Commit(params CatFileCommitOptions[] opts)
        {
            if(this.Repo == null) throw new Exception("Repository can not be null");
            return await this.Repo.CatFileCommit(this.CommitID.String(),opts);
        }

        
    }
}