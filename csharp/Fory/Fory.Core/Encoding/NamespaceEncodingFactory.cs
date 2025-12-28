using System;

namespace Fory.Core.Encoding
{
    public class NamespaceEncodingFactory : IEncodingFactory
    {
        private static readonly System.Text.Encoding Utf8Encoding = System.Text.Encoding.UTF8;
        private static readonly AllToLowerSpecialMetaStringEncoding AllToLowerSpecialEncoding =
            new AllToLowerSpecialMetaStringEncoding();
        private static readonly LowerUpperDigitSpecialMetaStringEncoding LowerUpperDigitSpecialEncoding =
            new LowerUpperDigitSpecialMetaStringEncoding();

        public static readonly Lazy<IEncodingFactory> Instance =
            new Lazy<IEncodingFactory>(() => new NamespaceEncodingFactory());

        public (System.Text.Encoding Encoding, byte Flag) GetEncoding(string value)
        {
            var stats = StringStatistics.GetStats(value);
            if (LowerUpperDigitSpecialEncoding.Evaluate(stats, value))
                return (LowerUpperDigitSpecialEncoding, 2);

            if (AllToLowerSpecialEncoding.Evaluate(stats, value))
                return (AllToLowerSpecialEncoding, 1);

            return (Utf8Encoding, 0);
        }
    }
}
