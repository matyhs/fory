using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.Serialization;
using Fory.Core.Spec.DataType;

namespace Fory.Core;

public sealed class DeserializationContext
{
    private readonly ForyOptions _options;
    private PipeReader? _reader;

    internal DeserializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeReader Reader => _reader ?? throw new SerializationException("Stream reader not initialized");

    public bool IsXlang => _options.Xlang;

    public bool IsCompatible => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

    internal void Initialize(Stream stream)
    {
        _reader = PipeReader.Create(stream);
    }
}
