using System;
using System.IO.Pipelines;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.Meta;

namespace Fory.Core
{
    public class SerializationContext
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

        public void Write()
        {
            // TODO: reserve the memory space at construction to reduce GC overhead.
            var span = _pipe.Writer.GetSpan(10);
            throw new NotImplementedException();
        }
    }
}
