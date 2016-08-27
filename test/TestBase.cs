using System;
using System.Text;
using SuperSocket.ProtoBase;
using System.Linq;
using Xunit;
using System.Collections.Generic;

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

        public void TestMultiplePackages<TReceiveFilter>(string packageLens, string pieces, Func<byte[], byte[]> dataEncoder)
            where TReceiveFilter : IReceiveFilter<StringPackageInfo>, new()
        {
            var arrPieces = pieces.Split(',');
            var piplelineProcessor = new DefaultPipelineProcessor<StringPackageInfo>(new TReceiveFilter());
            
            var arrLens = packageLens.Split(',');
            var bytes = new List<byte>();
            var sources = new List<string>();

            foreach(var len in arrLens)
            {
                var line = Guid.NewGuid().ToString().Substring(0, Int32.Parse(len));
                sources.Add(line);
                bytes.AddRange(dataEncoder(Encoding.UTF8.GetBytes(line)));
            }
            
            var packageData = bytes.ToArray();
            var totalLen = packageData.Length;
            var rest = totalLen;

            var resolvedPackages = new List<StringPackageInfo>();

            for (var i = 0; i < arrPieces.Length; i++)
            {
                var pieceTag = arrPieces[i];
                var currentLen = pieceTag == "*" ? rest : int.Parse(pieceTag);
                var result = piplelineProcessor.Process(new ArraySegment<byte>(packageData, totalLen - rest, currentLen));

                rest -= currentLen;

                if (result.Packages != null)
                    resolvedPackages.AddRange(result.Packages.OfType<StringPackageInfo>());
                
                if (rest <= 0) // last part
                {
                    Assert.Equal(ProcessState.Completed, result.State);
                }
            }

            Assert.Equal(arrLens.Length, resolvedPackages.Count);
            
            for (var i = 0; i < sources.Count; i++)
            {
                Assert.Equal(sources[i], resolvedPackages[i].Body);
            }
        }
    }
}