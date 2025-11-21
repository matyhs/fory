using System;
using System.Buffers.Binary;
using System.Threading.Tasks;

namespace Fory.Core.Spec
{
    /// <summary>
    /// Fory header specification: | magic number | reserved bits |  oob  | xlang | endian | null  |  language  | unsigned int for meta start offset |
    /// </summary>
    internal class ForyHeaderSpec : IForySpecDefinition
    {
        private const ushort MAGIC_NUMBER = 0x62d4;
        private const byte LANGUAGE_CODE = 0x8;

        [Flags]
        private enum ForyConfigFlags : byte
        {
            IsNull = 1,
            IsLittleEdian = 1 << 1,
            IsXlang = 1 << 2,
            IsOob = 1 << 3
        }

        public Task Serialize<TValue>(in TValue value, SerializationContext context)
        {
            var flag = ForyConfigFlags.IsLittleEdian;
            flag |= value is null ? ForyConfigFlags.IsNull : flag;
            flag |= context.IsXlang ? ForyConfigFlags.IsXlang : flag;

            // Fory header is 8 bytes in total
            var span = context.Writer.GetSpan(8);
            BinaryPrimitives.WriteUInt16LittleEndian(span, MAGIC_NUMBER);
            span.Fill((byte) flag);
            span.Fill(context.IsXlang ? LANGUAGE_CODE : (byte) 0);
            context.Writer.Advance(8);

            return Task.CompletedTask;
        }
    }
}
