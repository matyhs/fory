using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Utils;

namespace Fory.Core.Serializer;

internal sealed class TimestampSerializer : ForySerializerBase<DateTimeOffset>
{
    public override async Task SerializeDataAsync(DateTimeOffset value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var nanoseconds = (value - DateTimeUtils.GetUnixEpoch()).Ticks * 100;
        var span = context.Writer.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64LittleEndian(span, nanoseconds);
        context.Writer.Advance(sizeof(long));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<DateTimeOffset> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var readResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var sequence = readResult.Buffer.Slice(0, sizeof(long));
        var nanoseconds = BinaryPrimitives.ReadInt64LittleEndian(sequence.First.Span);
        var ticks = nanoseconds / 100 + DateTimeUtils.GetUnixEpoch().Ticks;
        context.Reader.AdvanceTo(sequence.End);

        return new DateTimeOffset(ticks, TimeSpan.Zero);
    }
}
