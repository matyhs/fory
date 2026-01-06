using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fory.Core.Serializer
{
    internal sealed class BooleanSerializer : ForySerializerBase<bool>
    {
        public override async Task SerializeDataAsync(bool value, SerializationContext context, CancellationToken cancellationToken = default)
        {
            var span = context.Writer.GetSpan(sizeof(byte));
            var byteValue = (byte) (value ? 1 : 0);
            MemoryMarshal.Write(span, ref byteValue);
            context.Writer.Advance(sizeof(byte));

            await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
