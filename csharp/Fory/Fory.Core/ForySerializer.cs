using System;
using Fory.Core.Spec;
using Fory.Core.Spec.DataType;

namespace Fory.Core
{
    public class ForySerializer
    {
        private static readonly IForySpecDefinition _headerSpec = new ForyHeaderSpec();
        private static readonly IForySpecDefinition _refMetaSpec = new ObjectRefMetaSpec();

        private readonly ForySerializerOptions _options;
        private readonly TypeSpecificationRegistry _typeSpecificationRegistry = new TypeSpecificationRegistry();

        public ForySerializer() : this(ForySerializerOptions.Default)
        {

        }

        public ForySerializer(ForySerializerOptions options)
        {
            _options = options;
        }

        public bool Register(Type type)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> Serialize<TValue>(TValue value)
        {
            var context = new SerializationContext(_options, _typeSpecificationRegistry);
            _headerSpec.Serialize(value, context);
            _refMetaSpec.Serialize(value, context);

            throw new NotImplementedException();
        }
    }
}
