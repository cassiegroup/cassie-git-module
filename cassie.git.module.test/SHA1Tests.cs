using System;
using Xunit;

namespace cassie.git.module.test
{
    public class SHA1Tests
    {
        [Fact]
        public void SHA1_Equal_TrueCases()
        {
            var s1 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Console.WriteLine(s1.String());

            Assert.True(s1.Equal("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4"));
            var s2 = SHA1.MustIDFromString("fcf7087e732bfe3c25328248a9bf8c3ccd85bed4");
            Assert.True(s2.Equal(s2.Bytes));
        }
    }
}
