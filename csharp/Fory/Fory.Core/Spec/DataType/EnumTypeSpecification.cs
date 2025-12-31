using System;
using System.Threading.Tasks;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType
{
    internal class EnumTypeSpecification<TEnum> : IEnumTypeSpecification
        where TEnum : Enum
    {
        public Type AssociatedType => typeof(TEnum);
        public uint TypeId { get; }
        public bool IsRegisteredByName { get; }
        public bool IsNamespaceIncluded { get; }
        public IForySerializer Serializer => EnumSerializer.Instance.Value;

        public EnumTypeSpecification(bool includeNamespace)
        {
            IsRegisteredByName = true;
            IsNamespaceIncluded = includeNamespace;
        }

        public EnumTypeSpecification(uint typeId)
        {
            TypeId = typeId;
            IsRegisteredByName = false;
        }

        public Task Serialize(object value, SerializationContext context)
        {
            Serializer.Serialize(value, context);
            return Task.CompletedTask;
        }
    }
}
