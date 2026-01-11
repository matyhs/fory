using System;

namespace Fory.Core.Encoding;

public class FieldNameEncodingFactory : IEncodingFactory
{
    private static readonly System.Text.Encoding Utf8Encoding = System.Text.Encoding.UTF8;

    private static readonly AllToLowerSpecialMetaStringEncoding AllToLowerSpecialEncoding = new();

    private static readonly LowerUpperDigitSpecialMetaStringEncoding LowerUpperDigitSpecialEncoding = new();

    public static readonly Lazy<IEncodingFactory> Instance = new(() => new FieldNameEncodingFactory());

    public (System.Text.Encoding Encoding, byte Flag) GetEncoding(string value)
    {
        var stats = StringStatistics.GetStats(value);
        if (LowerUpperDigitSpecialEncoding.Evaluate(stats, value))
            return (LowerUpperDigitSpecialEncoding, AsBitFieldNameEncoding(2));

        if (AllToLowerSpecialEncoding.Evaluate(stats, value))
            return (AllToLowerSpecialEncoding, AsBitFieldNameEncoding(1));

        return (Utf8Encoding, 0);
    }

    private static byte AsBitFieldNameEncoding(byte flag)
    {
        return (byte)(flag << 6);
    }
}
