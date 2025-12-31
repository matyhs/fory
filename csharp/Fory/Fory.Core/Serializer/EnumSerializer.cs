using System;
using System.Linq;
using System.Runtime.Serialization;
using Fory.Core.Encoding;

namespace Fory.Core.Serializer
{
    public sealed class EnumSerializer : IForySerializer
    {
        internal static readonly Lazy<IForySerializer> Instance = new Lazy<IForySerializer>(() => new EnumSerializer());

        public Type AssociatedType => typeof(Enum);

        public void Serialize<TValue>(TValue value, SerializationContext context)
        {
            if (!value.GetType().IsEnum)
            {
                throw new SerializationException($"Unable to serialize {value} using enum serializer");
            }

            var underlyingValue = GetUnderlyingValueAsUInt32(value);
            var converted = ForyEncoding.AsVarUInt32(underlyingValue).ToArray();
            var span = context.Writer.GetSpan(converted.Length);
            converted.CopyTo(span);
            context.Writer.Advance(converted.Length);
        }

        private static uint GetUnderlyingValueAsUInt32<TValue>(TValue value)
        {
            switch (value)
            {
                case sbyte s:
                    return (uint) s;
                case byte b:
                    return b;
                case short sh:
                    return (uint) sh;
                case ushort ush:
                    return ush;
                case int i:
                    return (uint) i;
                case uint ui:
                    return ui;
                case IntPtr intPtr:
                    return (uint) intPtr;
                case UIntPtr uIntPtr:
                    return (uint) uIntPtr;
                default:
                    throw new SerializationException($"The underlying integral numeric type for {value.GetType()} is not supported. Create a custom serializer to support the serialization of this enum.");
            }
        }
    }
}
