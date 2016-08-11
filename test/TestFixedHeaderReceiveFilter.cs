using System;
using System.Text;
using SuperSocket.ProtoBase;

namespace Tests
{
    public class TestFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringPackageInfo>
    {
        public TestFixedHeaderReceiveFilter()
            : base(2)
        {

        }
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return new StringPackageInfo(string.Empty, bufferStream.Skip(2).ReadString(Size - 2, Encoding.UTF8), null);
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var bodyLen = bufferStream.ReadInt16();
            return bodyLen;
        }
    }
}
