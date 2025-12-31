namespace Fory.Core.Serializer
{
    public sealed class BooleanSerializer : ForySerializerBase<bool>
    {
        public override void Serialize(bool value, SerializationContext context)
        {
            var span = context.Writer.GetSpan(1);
            var byteValue = (byte) (value ? 1 : 0);
            span.Fill(byteValue);
            context.Writer.Advance(1);
        }
    }
}
