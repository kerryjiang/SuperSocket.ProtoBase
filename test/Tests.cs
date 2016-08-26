using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public class Tests : TestBase
    {
        [Theory]
        [InlineData("*")]
        [InlineData("1,*")]
        [InlineData("1,1,*")]
        [InlineData("1,2,*")]
        [InlineData("5,6,7,8,*")]
        public void TestFixedHeaderSinglePackage(string pieces)
        {
            Func<byte[], byte[]> encoder = (d) =>
            {
                var bodyLen = d.Length;
                return (new byte[] { (byte)(bodyLen / 256), (byte)(bodyLen % 256) }).Concat(d).ToArray();
            };

            TestSinglePackage<TestFixedHeaderReceiveFilter>(pieces, encoder);
        }

        [Theory]
        [InlineData("*")]
        [InlineData("1,*")]
        [InlineData("1,1,*")]
        [InlineData("1,2,*")]
        [InlineData("5,6,7,8,*")]
        public void TestBeginEndMarkSinglePackage(string pieces)
        {
            Func<byte[], byte[]> encoder = (d) =>
            {
                var bodyLen = d.Length;
                return (new byte[] { (byte)'[' }).Concat(d).Concat(new byte[] { (byte)']' }).ToArray();
            };

            TestSinglePackage<TestBeginEndMarkReceiveFilter>(pieces, encoder);
        }
    }
}
