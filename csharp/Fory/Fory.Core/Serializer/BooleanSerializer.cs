using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fory.Core.Serializer;

internal sealed class BooleanSerializer : ForySerializerBase<bool>
{
    public override async Task SerializeDataAsync(bool value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var span = context.Writer.GetSpan(sizeof(byte));
        var byteValue = (byte)(value ? 1 : 0);
        MemoryMarshal.Write(span, ref byteValue);
        context.Writer.Advance(sizeof(byte));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<bool> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var readResult = await context.Reader.ReadAsync(cancellationToken);
        var sequence = readResult.Buffer.Slice(0, sizeof(byte));
        var value = MemoryMarshal.Read<bool>(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        return value;
    }
}
