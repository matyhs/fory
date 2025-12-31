using System;
using System.Threading.Tasks;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType
{
    internal class BooleanTypeSpecification : IKnownTypeSpecification<bool>
    {
        private readonly Lazy<IForySerializer<bool>> _serializer = new Lazy<IForySerializer<bool>>(() => new BooleanSerializer());

        public Type AssociatedType => typeof(bool);
        public uint TypeId => (uint) KnownTypeId;
        public TypeSpecificationRegistry.KnownTypes KnownTypeId => TypeSpecificationRegistry.KnownTypes.Boolean;
        IForySerializer ITypeSerializer.Serializer => Serializer;
        public IForySerializer<bool> Serializer => _serializer.Value;

        public Task Serialize(object value, SerializationContext context)
        {
            if (value is bool result)
            {
                return Serialize(result, context);
            }

            return Task.CompletedTask;
        }

        public Task Serialize(bool value, SerializationContext context)
        {
            Serializer.Serialize(value, context);
            return Task.CompletedTask;
        }
    }
}
