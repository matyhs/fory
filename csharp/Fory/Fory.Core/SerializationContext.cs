using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Spec;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.Meta;

namespace Fory.Core
{
    public sealed class SerializationContext
    {
        private readonly Pipe _pipe;
        private readonly ForyOptions _options;

        public PipeWriter Writer => _pipe.Writer;

        public PipeReader Reader => _pipe.Reader;

        public bool IsXlang => _options.Xlang;

        public bool ShareMeta => _options.Compatible;

        internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }
        internal TypeMetaRegistry TypeMetaRegistry { get; private set; }
        internal TypeMetaStringRegistry TypeMetaStringRegistry { get; private set; }

        internal SerializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
        {
            _pipe = new Pipe();
            TypeMetaRegistry = new TypeMetaRegistry(typeSpecificationRegistry);
            TypeMetaStringRegistry = new TypeMetaStringRegistry();

            _options = options;
            TypeSpecificationRegistry = typeSpecificationRegistry;
        }

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
        private readonly Pipe _pipe;
        private readonly ForyOptions _options;

        public PipeReader Reader => _pipe.Reader;

        public bool IsXlang => _options.Xlang;

        public bool ShareMeta => _options.Compatible;

        internal ForyHeaderSpec.ForyConfigFlags HeaderBitmap { get; set; }

        public byte SourceLanguageCode { get; set; }

        internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

        internal DeserializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
        {
            _pipe = new Pipe();

            _options = options;
            TypeSpecificationRegistry = typeSpecificationRegistry;
        }

        internal void Initialize(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                _pipe.Writer.Write(buffer.First.Span);
            }
            else
            {
                foreach (var segment in buffer)
                {
                    _pipe.Writer.Write(segment.Span);
                }
            }

            _pipe.Writer.Complete();
        }
    }
}
