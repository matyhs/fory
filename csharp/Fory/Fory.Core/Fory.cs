using System;
using Fory.Core.Spec;
using Fory.Core.Spec.DataType;

namespace Fory.Core
{
    public class Fory
    {
        private static readonly IForySpecDefinition _headerSpec = new ForyHeaderSpec();
        private static readonly IForySpecDefinition _refMetaSpec = new ObjectRefMetaSpec();

        private readonly ForyOptions _options;
        private readonly TypeSpecificationRegistry _typeSpecificationRegistry = new TypeSpecificationRegistry();

        public Fory() : this(ForyOptions.Default)
        {

        }

        public Fory(ForyOptions options)
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
