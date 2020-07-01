using System;
using Xunit;

namespace cassie.git.module.test
{
    public class SignatureTests
    {
        [Fact]
        public void SignatureOpr_Equal_TrueCases()
        {
            var line = "Patrick Gundlach <gundlach@speedata.de> 1378823654 +0200";
            var expSig = new Signature
            {
                Name = "Patrick Gundlach",
                Email = "gundlach@speedata.de",
                When = Utils.UnixTimeStampToDateTime(1378823654)
            };

            var sig = Signature.ParseSignature(line);

            Assert.True(expSig.Equals(sig));

            line = "Patrick Gundlach <gundlach@speedata.de> Tue Sep 10 16:34:14 2013 +0200";
            expSig = new Signature
            {
                Name = "Patrick Gundlach",
                Email = "gundlach@speedata.de",
                When = Utils.UnixTimeStampToDateTime(1378823654)
            };
            sig = Signature.ParseSignature(line);
            Assert.True(expSig.Equals(sig));
        }

    }
}