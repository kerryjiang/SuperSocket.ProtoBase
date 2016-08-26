using System;
using System.Text;
using SuperSocket.ProtoBase;
using System.Linq;
using Xunit;

namespace Tests
{
    public class TestBase
    {
        public void TestSinglePackage<TReceiveFilter>(string pieces, Func<byte[], byte[]> dataEncoder)
            where TReceiveFilter : IReceiveFilter<StringPackageInfo>, new()
        {
            var arrPieces = pieces.Split(',');
            var piplelineProcessor = new DefaultPipelineProcessor<StringPackageInfo>(new TReceiveFilter());
            
            var text = string.Join(" ", Enumerable.Range(1, 10).Select(x => Guid.NewGuid().ToString()).ToArray());
            var textData = Encoding.UTF8.GetBytes(text);
            var packageData = dataEncoder(textData);
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