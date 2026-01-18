using System;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fory.Core.Encoding;

internal static class ForyEncoding
{
    /// <summary>
    ///     Fory VarUInt32, consisting of 1~5 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
    ///     most significant bit (MSB) indicates the existence of another byte.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] AsVarUInt32(uint value)
    {
        Span<byte> span = stackalloc byte[5];
        var writtenBytes = WriteAsVarUInt32(value, span.Length, span);

        if (writtenBytes == 1)
            return span.Slice(0, 1).ToArray();

        if (writtenBytes == 2)
        {
            var converted = (ushort)(span[1] << 8 | span[0]);
            return BitConverter.GetBytes(converted);
        }

        if (writtenBytes == 3)
        {
            var converted = (ushort)(span[1] << 8 | span[0]);
            var bytes = BitConverter.GetBytes(converted);
            return bytes.Append(span[2]).ToArray();
        }

        if (writtenBytes == 4)
        {
            var converted = (uint)(span[3] << 24 | span[2] << 16 | span[1] << 8 | span[0]);
            return BitConverter.GetBytes(converted);
        }

        if (writtenBytes == 5)
        {
            var converted = (uint)(span[3] << 24 | span[2] << 16 | span[1] <<  8 | span[0]);
            var bytes = BitConverter.GetBytes(converted);
            return bytes.Append(span[4]).ToArray();
        }

        return [];
    }

    private static int WriteAsVarUInt32(uint value, int maxLength, Span<byte> buffer)
    {
        for (int i = 0; i < maxLength; i++)
        {
            if (value < 0x80)
            {
                buffer[i] = (byte)value;
                return i + 1;
            }

            buffer[i] = (byte)((value & 0x7f) | 0x80);
            value >>= 7;
        }

        return maxLength;
    }

    /// <summary>
    ///     Fory VarUInt32, consisting of 1~5 bytes, decodes buffer by reading a 7-bit grouping per byte. The
    ///     most significant bit (MSB) is disregarded from the decoded value.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<uint> FromVarUInt32Async(PipeReader reader,
        CancellationToken cancellationToken = default)
    {
        var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var sequence = readResult.Buffer.Slice(0, sizeof(byte));
        var local = MemoryMarshal.Read<byte>(sequence.First.Span);
        reader.AdvanceTo(sequence.End);

        if (local < 0x80)
            return local;

        var value = (uint)(local & 0x7f);
        for (uint i = 1; i < 5; i++)
        {
            readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            sequence = readResult.Buffer.Slice(0, sizeof(byte));
            local = MemoryMarshal.Read<byte>(sequence.First.Span);
            value |= (uint)(local & 0x7f) << 7 * (int)i;
            reader.AdvanceTo(sequence.End);

            if (local < 0x80)
                break;
        }

        return value;
    }
}
