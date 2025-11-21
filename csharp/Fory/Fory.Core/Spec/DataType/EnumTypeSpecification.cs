using System;
using System.Threading.Tasks;

namespace Fory.Core.Spec.DataType
{
    internal class EnumTypeSpecification<TEnum> : IEnumTypeSpecification
        where TEnum : Enum
    {
        public Type AssociatedType => typeof(TEnum);

        public uint TypeId { get; }

        public bool IsRegisteredByName { get; }

        public bool IsNamespaceIncluded { get; }

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
            throw new NotImplementedException();
        }

    }
}
