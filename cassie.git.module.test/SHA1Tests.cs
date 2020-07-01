using System;
using System.Text;
using Xunit;

namespace cassie.git.module.test
{
    public class SHA1Tests
    {
        [Fact]
        public void SHA1_Equal_TrueCases()
        {
            var s1 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Assert.True(s1.Equal("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4"));
            var s2 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            var s3 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Assert.True(s2.Equal(s3.Bytes));
            var s4 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            var s5 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Assert.True(s4.Equal(s5));
        }

        [Fact]
        public void SHA1_Equal_FalseCases()
        {
            var s1 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Assert.False(s1.Equal(SHA1.EmptyID));
            var s2 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            var s3 = SHA1.MustIDFromString(SHA1.EmptyID);
            Assert.False(s2.Equal(s3.Bytes));
            var s4 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            var s5 = SHA1.MustIDFromString(SHA1.EmptyID);
            Assert.False(s4.Equal(s5));
        }

        [Fact]
        public void SHA1_NewID_EqualCases()
        {
            Assert.Throws<LengthNotMatchException>(()=>SHA1.NewID(Encoding.ASCII.GetBytes("000000")));
        }
    }
}
