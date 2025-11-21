using System.Collections.Generic;

namespace Fory.Core.Encoding
{
    internal static class ForyEncoding
    {
        /// <summary>
        /// Fory VarInt32, consisting of 1~5 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
        /// most significant bit (MSB) indicates the existence of another byte.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<byte> AsVarInt32(uint value)
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
    }
}
