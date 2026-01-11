using System;

namespace Fory.Core.Encoding;

public class TypeNameEncodingFactory : IEncodingFactory
{
    private static readonly System.Text.Encoding Utf8Encoding = System.Text.Encoding.UTF8;

    private static readonly FirstToLowerSpecialMetaStringEncoding FirstToLowerSpecialEncoding = new();

    private static readonly AllToLowerSpecialMetaStringEncoding AllToLowerSpecialEncoding = new();

    private static readonly LowerUpperDigitSpecialMetaStringEncoding LowerUpperDigitSpecialEncoding = new();

    public static readonly Lazy<IEncodingFactory> Instance = new(() => new TypeNameEncodingFactory());

    public (System.Text.Encoding Encoding, byte Flag) GetEncoding(string value)
    {
        var stats = StringStatistics.GetStats(value);
        if (LowerUpperDigitSpecialEncoding.Evaluate(stats, value))
            return (LowerUpperDigitSpecialEncoding, 2);

        if (FirstToLowerSpecialEncoding.Evaluate(stats, value))
            return (FirstToLowerSpecialEncoding, 3);

        if (AllToLowerSpecialEncoding.Evaluate(stats, value))
            return (AllToLowerSpecialEncoding, 1);

        return (Utf8Encoding, 0);
    }
}
