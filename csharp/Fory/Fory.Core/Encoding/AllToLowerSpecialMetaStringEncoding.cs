using System.Buffers;
using System.Text;

namespace Fory.Core.Encoding
{
    internal class AllToLowerSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
    {
        private static readonly Encoder DefaultEncoder = new FiveBitMetaStringEncoder();

        public override int GetByteCount(char[] chars, int index, int count)
        {
            var pool = ArrayPool<byte>.Shared;
            var rent = pool.Rent(count * 2);
            var byteCount = GetBytes(chars, index, count, rent, 0);
            pool.Return(rent, true);

            return byteCount;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var pool = ArrayPool<char>.Shared;
            // Rent at least two times the input array size, making sure we have enough space in case all characters are capitalized.
            var rent = pool.Rent(charCount * 2);
            var pos = 0;
            foreach (var c in chars)
            {
                if (char.IsUpper(c))
                {
                    rent[pos++] = '|';
                    rent[pos++] = char.ToLower(c);
                    continue;
                }

                rent[pos++] = c;
            }

            var written = DefaultEncoder.GetBytes(rent, charIndex, charCount, bytes, byteIndex, false);
            pool.Return(rent, true);

            return written;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new System.NotImplementedException();
        }

        public override int GetMaxByteCount(int charCount)
        {
            throw new System.NotImplementedException();
        }

        public override int GetMaxCharCount(int byteCount)
        {
            throw new System.NotImplementedException();
        }

        public bool Evaluate(StringStatistics stats, string value)
        {
            return stats.CanLowerUpperDigitSpecialEncode && (value.Length + stats.UpperCount) * 5 < value.Length * 6;
        }
    }
}
