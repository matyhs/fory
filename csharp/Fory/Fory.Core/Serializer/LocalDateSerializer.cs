using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Utils;

namespace Fory.Core.Serializer;

internal sealed class LocalDateSerializer : ForySerializerBase<DateTime>
{
    public override async Task SerializeDataAsync(DateTime value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var days = (int)((value - DateTimeUtils.GetUnixEpoch().Date).Ticks / TimeSpan.TicksPerDay);
        var span = context.Writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, days);
        context.Writer.Advance(sizeof(int));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<DateTime> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var readResult = await context.Reader.ReadAsync(cancellationToken);
        var sequence = readResult.Buffer.Slice(0, sizeof(int));
        var value = BinaryPrimitives.ReadInt32LittleEndian(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        return DateTimeUtils.GetUnixEpoch().DateTime.AddDays(value);
    }
}
