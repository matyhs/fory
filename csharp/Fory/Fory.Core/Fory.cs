using System;
using System.Buffers;
using Fory.Core.Serializer;
using Fory.Core.Spec;
using Fory.Core.Spec.DataType;

namespace Fory.Core
{
    public class Fory
    {
        private static readonly IForySpecDefinition HeaderSpec = new ForyHeaderSpec();
        private static readonly IForySpecDefinition RefMetaSpec = new ObjectRefMetaSpec();
        private static readonly IForySpecDefinition TypeMetaSpec = new ObjectTypeMetaSpec();
        private static readonly IForySpecDefinition ValueSpec = new ForyValueSpec();

        private readonly ForyOptions _options;
        private readonly TypeSpecificationRegistry _typeSpecificationRegistry = new TypeSpecificationRegistry();

        public Fory() : this(ForyOptions.Default)
        {

        }

        public Fory(ForyOptions options)
        {
            _options = options;
        }

        public bool Register<TObject>()
        {
            throw new NotImplementedException();
        }

        public bool RegisterSerializer<TSerializer>() where TSerializer : IForySerializer
        {
            throw new NotImplementedException();
        }

        public ReadOnlySequence<byte> Serialize<TValue>(TValue value)
        {
            var context = new SerializationContext(_options, _typeSpecificationRegistry);
            HeaderSpec.Serialize(value, context);
            RefMetaSpec.Serialize(value, context);
            TypeMetaSpec.Serialize(value, context);
            ValueSpec.Serialize(value, context);

            context.Writer.Complete();
            context.Reader.TryRead(out var readResult);

            return readResult.Buffer;
        }
    }
}
