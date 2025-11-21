using System.Threading.Tasks;

namespace Fory.Core.Spec
{
    internal class ObjectRefMetaSpec : IForySpecDefinition
    {
        private enum ReferenceFlag : sbyte
        {
            // This flag indicates the object is a null value. We don't use another byte to indicate REF, so that we can save one byte.
            Null = -3,
            // This flag indicates the object is already serialized previously, and fory will write a ref id with unsigned varint format instead of serialize it again
            Ref = -2,
            // This flag indicates the object is a non-null value and fory doesn't track ref for this type of object.
            NotNull = -1,
            // This flag indicates the object is referencable and the first time to serialize.
            RefValue = 0
        }

        public Task Serialize<TValue>(in TValue value, SerializationContext context)
        {
            var refFlag = value is null ? ReferenceFlag.Null : ReferenceFlag.NotNull;

            var span = context.Writer.GetSpan(1);
            span.Fill((byte)(sbyte)refFlag);
            context.Writer.Advance(1);

            return Task.CompletedTask;
        }
    }
}
