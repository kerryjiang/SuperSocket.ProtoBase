using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public class Tests : TestBase
    {
        private Func<byte[], byte[]> GetFixedHeaderEncoder()
        {
            Func<byte[], byte[]> encoder = (d) =>
            {
                var bodyLen = d.Length;
                return (new byte[] { (byte)(bodyLen / 256), (byte)(bodyLen % 256) }).Concat(d).ToArray();
            };

            return encoder;
        }

        [Theory]
        [InlineData("*")]
        [InlineData("1,*")]
        [InlineData("1,1,*")]
        [InlineData("1,2,*")]
        [InlineData("5,6,7,8,*")]
        public void TestFixedHeaderSinglePackage(string pieces)
        {
            TestSinglePackage<TestFixedHeaderReceiveFilter>(pieces, GetFixedHeaderEncoder());
        }

        [Theory]
        [InlineData("1,10,15", "*")]
        [InlineData("1,10,15", "1,1,*")]
        [InlineData("1,10,15", "10,2,*")]
        public void TestFixedHeaderMultiplePackages(string packageLens, string pieces)
        {
            TestMultiplePackages<TestFixedHeaderReceiveFilter>(packageLens, pieces, GetFixedHeaderEncoder());
        }

        private Func<byte[], byte[]> GetBeginEndMarkEncoder()
        {
            Func<byte[], byte[]> encoder = (d) =>
            {
                var bodyLen = d.Length;
                return (new byte[] { (byte)'[' }).Concat(d).Concat(new byte[] { (byte)']' }).ToArray();
            };

            return encoder;
        }

        [Theory]
        [InlineData("*")]
        [InlineData("1,*")]
        [InlineData("1,1,*")]
        [InlineData("1,2,*")]
        [InlineData("5,6,7,8,*")]
        public void TestBeginEndMarkSinglePackage(string pieces)
        {
            TestSinglePackage<TestBeginEndMarkReceiveFilter>(pieces, GetBeginEndMarkEncoder());
        }

        [Theory]
        [InlineData("1,10,15", "*")]
        [InlineData("1,10,15", "1,1,*")]
        [InlineData("1,10,15", "10,2,*")]
        public void TestBeginEndMarkMultiplePackages(string packageLens, string pieces)
        {
            TestMultiplePackages<TestBeginEndMarkReceiveFilter>(packageLens, pieces, GetBeginEndMarkEncoder());
        }
    }
}
