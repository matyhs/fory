using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Encoding;

namespace Fory.Core.Serializer;

public sealed class StringSerializer : ForySerializerBase<string>
{
    /// <summary>
    ///     Serializes string using UTF-8 encoding. See https://learn.microsoft.com/en-us/dotnet/api/System.Text.Encoding.Default?view=netstandard-2.0.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task SerializeDataAsync(string value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        const byte utf8EncodingType = 2;
        var buffer = System.Text.Encoding.Default.GetBytes(value);
        var header = ForyEncoding.AsVarUInt36Small((ulong)((buffer.Length << 2) | utf8EncodingType)).ToArray();

        var span = context.Writer.GetSpan(header.Length);
        header.CopyTo(span);
        context.Writer.Advance(header.Length);

        span = context.Writer.GetSpan(buffer.Length);
        buffer.CopyTo(span);
        context.Writer.Advance(buffer.Length);

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<string> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var header = await ForyEncoding.FromVarUInt36Async(context.Reader, cancellationToken);
        var encoding = (byte)(header & 0x03);
        var length = (long)(header >> 2);
        var readResult = await context.Reader.ReadAsync(cancellationToken);
        var sequence = readResult.Buffer.Slice(0, length);

        return encoding switch
        {
#if NET
            0 or 1 => throw new NotImplementedException(),
            2 => System.Text.Encoding.UTF8.GetString(sequence.First.Span),
#else
            0 or 1 => throw new NotImplementedException(),
            2 => System.Text.Encoding.UTF8.GetString(sequence.First.Span.ToArray()),
#endif
            _ => throw new NotSupportedException("Unsupported string encoding type")
        };
    }
}
