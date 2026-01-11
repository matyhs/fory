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

    public readonly record struct HeaderInfo
    {
        private readonly ForyHeaderSpec.ForyConfigFlags _bitmap;

        public byte? SourceLanguageCode { get; }

        public bool IsPeerXlang => _bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsXlang);

        public bool IsPeerLittleEdian => _bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsLittleEdian);

        public bool IsNull => _bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsNull);

        internal HeaderInfo(ForyHeaderSpec.ForyConfigFlags bitmap)
        {
            _bitmap = bitmap;
        }

        internal HeaderInfo(ForyHeaderSpec.ForyConfigFlags bitmap, byte languageCode) : this(bitmap)
        {
            SourceLanguageCode = languageCode;
        }
    }
}
