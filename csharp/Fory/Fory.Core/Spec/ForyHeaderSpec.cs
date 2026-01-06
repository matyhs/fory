using System;

namespace Fory.Core.Spec
{
    internal class ForyHeaderSpec
    {
        public const ushort MagicNumber = 0x62d4;
        public const byte LanguageCode = 0x8;

        [Flags]
        public enum ForyConfigFlags : byte
        {
            IsNull = 1,
            IsLittleEdian = 1 << 1,
            IsXlang = 1 << 2,
            IsOob = 1 << 3
        }
    }
}
