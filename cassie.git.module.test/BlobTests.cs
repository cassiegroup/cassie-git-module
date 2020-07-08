using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.tree;
using Xunit;

namespace cassie.git.module.test
{
    public class BlobTests
    {
        [Fact]
        public void Blob_Bytes()
        {
            var expOutput = @"This is a sample project students can use during Matthew's Git class.

Here is an addition by me

We can have a bit of fun with this repo, knowing that we can always reset it to a known good state.  We can apply labels, and branch, then add new code and merge it in to the master branch.

As a quick reminder, this came from one of three locations in either SSH, Git, or HTTPS format:

*git@github.com:matthewmccullough / hellogitworld.git
* git://github.com/matthewmccullough/hellogitworld.git
*https://matthewmccullough@github.com/matthewmccullough/hellogitworld.git

We can, as an example effort, even modify this README and change it as if it were source code for the purposes of the class.

This demo also includes an image with changes on a branch for examination of image diff on GitHub.
";      
            var te = new TreeEntry();
            te.Mode = EntryMode.EntryBlob;
            te.Typ = ObjectType.ObjectBlob;
            te.ID = SHA1.MustIDFromString("adfd6da3c0a3fb038393144becbf37f14f780087");
            te.Parent = new Tree{

            };
            //var blob = new Blob();
            
        }

        
    }
}