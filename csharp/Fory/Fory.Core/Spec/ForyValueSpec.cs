using System;
using System.Threading.Tasks;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core.Spec
{
    public class ForyValueSpec : IForySpecDefinition
    {
        public Task Serialize<TValue>(in TValue value, SerializationContext context)
        {
            if (value is null)
                return Task.CompletedTask;

            var typeSpec = context.TypeSpecificationRegistry.GetTypeSpecification(value.GetType());
            if (typeSpec is IKnownTypeSpecification<TValue> knownTypeMetaSpec)
            {
                knownTypeMetaSpec.Serialize(value, context);
                return Task.CompletedTask;
            }

            typeSpec.Serialize(value, context);
            return Task.CompletedTask;
        }
    }
}
