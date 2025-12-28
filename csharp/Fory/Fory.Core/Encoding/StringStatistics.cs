using System.Linq;

namespace Fory.Core.Encoding
{
    internal struct StringStatistics
    {
        public uint DigitCount { get; private set; }
        public uint UpperCount { get; private set; }
        public bool CanLowerUpperDigitSpecialEncode { get; private set; }
        public bool CanLowerSpecialEncode { get; private set; }

        internal static StringStatistics GetStats(string value, params char[] specialCharacters)
        {
            var stats = new StringStatistics()
            {
                CanLowerSpecialEncode = true,
                CanLowerUpperDigitSpecialEncode = true
            };

            foreach (var c in value)
            {
                if (stats.CanLowerUpperDigitSpecialEncode &&
                    !(char.IsLower(c) || char.IsUpper(c) || char.IsDigit(c) || specialCharacters.Contains(c)))
                    stats.CanLowerUpperDigitSpecialEncode = false;

                if (stats.CanLowerSpecialEncode &&
                    !(char.IsLower(c) || c == '.' || c == '_' || c == '$' || c == '|'))
                    stats.CanLowerSpecialEncode = false;

                if (char.IsDigit(c))
                    stats.DigitCount++;

                if (char.IsUpper(c))
                    stats.UpperCount++;
            }

            return stats;
        }
    }
}
