using System;
using System.Security.Cryptography;

using Xunit;

namespace CoreUpdater.Common.Tests
{
    public class HashTest
    {
        [Fact]
        public void ExceptionTest()
        {
            string str = null;
            Assert.Throws<ArgumentNullException>(() => Hash.GetHash<SHA256CryptoServiceProvider>(str));
        }

        [Theory,
            InlineData("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", "test")]
        public void Sha256Test(string expected, string str)
        {
            Assert.Equal(expected, Hash.GetHash<SHA256CryptoServiceProvider>(str));
        }
    }
}
