using System.Collections.Generic;

namespace Fory.Core.Encoding
{
    internal static class EncodingRegistry
    {
        public static readonly IReadOnlyDictionary<string, uint> SupportedFieldNameEncodings = new Dictionary<string, uint>()
        {
            { System.Text.Encoding.UTF8.EncodingName, 0x01 },
            { System.Text.Encoding.UTF8.EncodingName, 0x01 },
            { System.Text.Encoding.UTF8.EncodingName, 0x01 }
        };
    }
}
