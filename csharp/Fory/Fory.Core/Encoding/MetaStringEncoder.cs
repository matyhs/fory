using System;
using System.Text;

namespace Fory.Core.Encoding
{
    public abstract class MetaStringEncoder : Encoder
    {
        private readonly ushort _bitsPerCharacter;

        protected MetaStringEncoder(ushort bitsPerCharacter)
        {
            _bitsPerCharacter = bitsPerCharacter;
        }

        protected abstract ushort GetCharValue(char c);

        public override int GetByteCount(char[] chars, int index, int count, bool flush)
        {
            return GetByteLength(count, _bitsPerCharacter);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex,
            bool flush)
        {
            if (bytes.Length != charCount)
                bytes = new byte[charCount];

            var currBit = 1;
            // represent the character value as a contiguous byte
            for (var i = 0; i < charCount; i++)
            {
                var value = GetCharValue(chars[i]);
                for (var j = _bitsPerCharacter - 1; j >= 0; j--)
                {
                    var hasValue = (value & (1 << j)) != 0;
                    if (hasValue)
                    {
                        var bytePos = currBit / 8;
                        var bitPos = currBit % 8;
                        bytes[bytePos] |= (byte)(1 << (7 - bitPos));
                    }

                    currBit += 1;
                }
            }

            var bitCount = chars.Length * _bitsPerCharacter + 1;
            var addStripFlag = GetByteLength(charCount, _bitsPerCharacter) * 8 >= bitCount;
            if (addStripFlag)
                bytes[0] |= 0x80;

            return currBit / 8;
        }

        private static int GetByteLength(int charCount, ushort bitsPerChar)
        {
            return (charCount * bitsPerChar + 8) / 8;
        }
    }

    public class FiveBitMetaStringEncoder : MetaStringEncoder
    {
        public FiveBitMetaStringEncoder() : base(5)
        {
        }

        protected override ushort GetCharValue(char c)
        {
            var result = c - 'a';
            if (result >= 0 && result <= 25)
                return (ushort) result;

            switch (c)
            {
                case '.':
                    return 26;
                case '_':
                    return 27;
                case '$':
                    return 28;
                case '|':
                    return 29;
            }

            throw new NotSupportedException($"Character '{c}' cannot be represented as a 5-bit value.");
        }
    }

    public class SixBitMetaStringEncoder : MetaStringEncoder
    {
        private readonly char[] _specialCharacters;

        public SixBitMetaStringEncoder(params char[] specialCharacters) : base(6)
        {
            _specialCharacters = specialCharacters;
        }

        protected override ushort GetCharValue(char c)
        {
            var result = c - 'a';
            if (result >= 0 && result <= 25)
                return (ushort) result;

            result = c - 'A';
            if (result >= 0 && result <= 25)
                return (ushort)(result + 26);

            var number = char.GetNumericValue(c);
            if (number >= 0)
                return (ushort)(number + 52);

            if (c == _specialCharacters[0])
                return 62;

            if (c == _specialCharacters[1])
                return 63;

            throw new NotSupportedException($"Character '{c}' cannot be represented as a 6-bit value.");
        }
    }
}
