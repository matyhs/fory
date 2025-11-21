using System;
using System.IO.Pipelines;
using Fory.Core.Spec.DataType;

namespace Fory.Core
{
    internal class SerializationContext
    {
        private readonly Pipe _pipe;
        private readonly ForySerializerOptions _options;

        public PipeWriter Writer => _pipe.Writer;

        public PipeReader Reader => _pipe.Reader;

        public bool IsXlang => _options.Xlang;

        public bool ShareMeta => _options.Compatible;

        public TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

        public SerializationContext(ForySerializerOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
        {
            _pipe = new Pipe();
            _options = options;
            TypeSpecificationRegistry = typeSpecificationRegistry;
        }

        public void Write()
        {
            // TODO: reserve the memory space at construction to reduce GC overhead.
            var span = _pipe.Writer.GetSpan(10);
            throw new NotImplementedException();
        }
    }
}
