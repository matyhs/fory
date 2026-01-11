using System;
using System.Text;

namespace Fory.Core.Encoding;

internal class LowerSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
{
    private static readonly Encoder DefaultEncoder = new FiveBitMetaStringEncoder();

    public bool Evaluate(StringStatistics stats, string value)
    {
        throw new NotImplementedException();
    }

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
        return DefaultEncoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex, false);
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
