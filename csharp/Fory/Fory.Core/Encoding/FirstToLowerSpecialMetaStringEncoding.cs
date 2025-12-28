using System.Buffers;
using System.Text;

namespace Fory.Core.Encoding
{
    internal class FirstToLowerSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
    {
        private static readonly Encoder DefaultEncoder = new FiveBitMetaStringEncoder();

        public override Encoder GetEncoder()
        {
            return new FiveBitMetaStringEncoder();
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return DefaultEncoder.GetByteCount(chars, index, count, false);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var pool = ArrayPool<char>.Shared;
            var rent = pool.Rent(charCount);
            for (var i = 0; i < chars.Length; i++)
            {
                rent[i] = i == 0 ? char.ToLower(chars[i]) : chars[i];
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
            return stats.UpperCount == 1 && char.IsUpper(value[0]);
        }
    }
}
