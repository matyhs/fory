using System;
using System.Threading.Tasks;

namespace Fory.Core.Spec.DataType
{
    internal class BooleanTypeSpecification : IKnownTypeSpecification
    {
        public Type AssociatedType => typeof(bool);

        public uint TypeId => (uint) KnownTypeId;

        public TypeSpecificationRegistry.KnownTypes KnownTypeId => TypeSpecificationRegistry.KnownTypes.Boolean;

        public Task Serialize(object value, SerializationContext context)
        {
            if (value is bool result)
            {

            }

            return Task.CompletedTask;
        }
    }
}
