using System;
using System.Text;

namespace Fory.Core.Encoding;

internal sealed class LowerUpperDigitSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
{
    private readonly Encoder _encoder;
    private readonly char[] _specialCharacters;

    public LowerUpperDigitSpecialMetaStringEncoding(params char[] specialCharacters)
    {
        _specialCharacters = specialCharacters;
        _encoder = new SixBitMetaStringEncoder(specialCharacters);
    }

    public bool Evaluate(StringStatistics stats, string _)
    {
        return stats.CanLowerUpperDigitSpecialEncode && stats.DigitCount > 0;
    }

    public override Encoder GetEncoder()
    {
        return new SixBitMetaStringEncoder(_specialCharacters);
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        return _encoder.GetByteCount(chars, index, count, false);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        return _encoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex, false);
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxByteCount(int charCount)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxCharCount(int byteCount)
    {
        throw new NotImplementedException();
    }
}
