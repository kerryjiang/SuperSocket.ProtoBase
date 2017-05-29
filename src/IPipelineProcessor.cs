using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSocket.ProtoBase
{
    /// <summary>
    /// The pipeline data processor
    /// </summary>
    public interface IPipelineProcessor
    {
        /// <summary>
        /// Processes the input segment.
        /// </summary>
        /// <param name="segment">The input segment.</param>
        /// <returns>the processing result</returns>
        ProcessResult Process(ArraySegment<byte> segment);

        /// <summary>
        /// Gets the received cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        BufferList Cache { get; }

        /// <summary>
        /// Reset the state of the processor to the initial status
        /// </summary>
        void Reset();
    }
}
