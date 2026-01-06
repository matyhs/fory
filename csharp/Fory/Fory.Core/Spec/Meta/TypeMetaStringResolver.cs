using System;
using System.Buffers.Binary;
using System.Linq;
using Fory.Core.Encoding;

namespace Fory.Core.Spec.Meta
{
    internal static class TypeMetaStringResolver
    {
        /// <summary>
        /// Meta string encoding with deduplication
        /// </summary>
        /// <param name="index"></param>
        /// <param name="metaStringBytes"></param>
        /// <returns></returns>
        public static byte[] Encode(uint index, MetaStringBytes? metaStringBytes)
        {
            if (metaStringBytes is null)
                return EncodeReference(index);

            return EncodeFirstOccurence(metaStringBytes);
        }

        /// <summary>
        /// | ((id + 1) shift left 1) | 1 |
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static byte[] EncodeReference(uint index)
        {
            index = ((index + 1) << 1) | 1;
            return ForyEncoding.AsVarUInt32(index).ToArray();
        }

        /// <summary>
        /// | (length shift left 1) | [hash if large] | encoding | bytes |
        /// </summary>
        /// <param name="metaStringBytes"></param>
        /// <returns></returns>
        private static byte[] EncodeFirstOccurence(MetaStringBytes metaStringBytes)
        {
            const byte smallStringThreshold = 16;
            var length = metaStringBytes.Bytes.Length;
            var buffer = ForyEncoding.AsVarUInt32((uint) length << 1).ToArray();
            var writtenBytes = buffer.Length;
            if (length > smallStringThreshold)
            {
                Array.Resize(ref buffer, buffer.Length + length + 8);
                var span = buffer.AsSpan(writtenBytes, 8);
                BinaryPrimitives.WriteInt64LittleEndian(span, metaStringBytes.HashCode);
                writtenBytes += 8;
            }
            else
            {
                Array.Resize(ref buffer, buffer.Length + length + 1);
                var span = buffer.AsSpan(writtenBytes, 1);
                span.Fill(metaStringBytes.Encoding);
                writtenBytes += 1;
            }
            metaStringBytes.Bytes.CopyTo(buffer, writtenBytes);

            return buffer;
        }
    }
}
