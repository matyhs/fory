using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fory.Core.Encoding
{
    internal static class ForyEncoding
    {
        /// <summary>
        /// Fory VarUInt32, consisting of 1~5 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
        /// most significant bit (MSB) indicates the existence of another byte.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<byte> AsVarUInt32(uint value)
        {
            for (uint i = 0; i < 5; i++)
            {
                if (value < 0x80)
                {
                    yield return (byte)value;
                    yield break;
                }

                yield return (byte)(value & 0x7f | 0x80);
                value >>= 7;
            }
        }

        /// <summary>
        /// Fory VarUInt32, consisting of 1~5 bytes, decodes buffer by reading a 7-bit grouping per byte. The
        /// most significant bit (MSB) is disregarded from the decoded value.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async ValueTask<uint> FromVarUInt32Async(PipeReader reader,
            CancellationToken cancellationToken = default)
        {
            uint value = 0;
            for (uint i = 0; i < 5; i++)
            {
                var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var sequence = readResult.Buffer.Slice(0, sizeof(byte));
                var local = MemoryMarshal.Read<byte>(sequence.First.Span);
                value <<= 7;
                value |= (byte)(local & 0x7f);
                reader.AdvanceTo(sequence.End);

                if (local < 0x80)
                    break;
            }

            return value;
        }
    }
}
