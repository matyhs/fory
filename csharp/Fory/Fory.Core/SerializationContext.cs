using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.Meta;

namespace Fory.Core;

public sealed class SerializationContext
{
    private readonly ForyOptions _options;
    private readonly Pipe _pipe;

    internal SerializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _pipe = new Pipe();
        TypeMetaRegistry = new TypeMetaRegistry(typeSpecificationRegistry);
        TypeMetaStringRegistry = new TypeMetaStringRegistry();

        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeWriter Writer => _pipe.Writer;

    public PipeReader Reader => _pipe.Reader;

    public bool IsXlang => _options.Xlang;

    public bool ShareMeta => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }
    internal TypeMetaRegistry TypeMetaRegistry { get; private set; }
    internal TypeMetaStringRegistry TypeMetaStringRegistry { get; private set; }

    internal async ValueTask<ReadOnlySequence<byte>> CompleteAsync(CancellationToken cancellationToken = default)
    {
        await Writer.CompleteAsync().ConfigureAwait(false);
        if (Reader.TryRead(out var readResult))
            return readResult.Buffer;

        throw new SerializationException("Error occurred while accessing buffer data");
    }
}

public sealed class DeserializationContext
{
    private readonly ForyOptions _options;
    private readonly Pipe _pipe;

    internal DeserializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _pipe = new Pipe();

        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeReader Reader => _pipe.Reader;

    public bool IsXlang => _options.Xlang;

    public bool ShareMeta => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

    internal void Initialize(ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsSingleSegment)
            _pipe.Writer.Write(buffer.First.Span);
        else
            foreach (var segment in buffer)
                _pipe.Writer.Write(segment.Span);

        _pipe.Writer.Complete();
    }
}
