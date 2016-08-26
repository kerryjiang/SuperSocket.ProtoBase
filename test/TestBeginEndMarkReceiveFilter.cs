using System;
using System.Text;
using SuperSocket.ProtoBase;

namespace Tests
{
    public class TestBeginEndMarkReceiveFilter : BeginEndMarkReceiveFilter<StringPackageInfo>
    {
        public TestBeginEndMarkReceiveFilter()
            : base(new byte[] { (byte)'[' }, new byte[] { (byte)']' })
        {

        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return new StringPackageInfo(string.Empty, bufferStream.Skip(1).ReadString((int)bufferStream.Length - 2, Encoding.UTF8), null);
        }
    }
}
