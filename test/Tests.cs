using System;
using System.Text;
using SuperSocket.ProtoBase;
using System.Linq;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Theory]
        [InlineData("*")]
        [InlineData("1,*")]
        [InlineData("1,1,*")]
        [InlineData("1,2,*")]
        [InlineData("5,6,7,8,*")]
        public void TestFixedHeaderReceiveFilter(string pieces) 
        {
            var arrPieces = pieces.Split(',');
            var piplelineProcessor = new DefaultPipelineProcessor<StringPackageInfo>(new TestFixedHeaderReceiveFilter());
            
            var text = string.Join(" ", Enumerable.Range(1, 10).Select(x => Guid.NewGuid().ToString()).ToArray());
            var textData = Encoding.UTF8.GetBytes(text);
            var bodyLen = textData.Length;

            var packageData = (new byte[] { (byte)(bodyLen / 256), (byte)(bodyLen % 256) }).Concat(textData).ToArray();
            var totalLen = packageData.Length;
            var rest = totalLen;

            for (var i = 0; i < arrPieces.Length; i++)
            {
                var pieceTag = arrPieces[i];
                var currentLen = pieceTag == "*" ? rest : int.Parse(pieceTag);
                var result = piplelineProcessor.Process(new ArraySegment<byte>(packageData, totalLen - rest, currentLen));

                rest -= currentLen;
                
                if (rest > 0)
                {
                    Assert.Equal(ProcessState.Cached, result.State);
                }
                else // last part
                {
                    Assert.Equal(ProcessState.Completed, result.State);
                    Assert.Equal(1, result.Packages.Count);
                    Assert.Equal(text, ((StringPackageInfo)result.Packages[0]).Body);
                }
            }            
        }
    }
}
