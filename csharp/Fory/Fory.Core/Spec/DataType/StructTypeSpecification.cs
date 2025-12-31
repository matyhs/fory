using System;
using System.Threading.Tasks;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType
{
    internal class StructTypeSpecification<TObject> : IStructTypeSpecification
    {
        public Type AssociatedType => typeof(TObject);

        public uint TypeId { get; }

        public bool IsRegisteredByName { get; }

        public bool IsNamespaceIncluded { get; }

        public StructTypeSpecification(bool includeNamespace)
        {
            IsRegisteredByName = true;
            IsNamespaceIncluded = includeNamespace;
        }

        public StructTypeSpecification(uint typeId)
        {
            TypeId = typeId;
            IsRegisteredByName = false;
        }

        public Task Serialize(object value, SerializationContext context)
        {
            throw new NotImplementedException();
        }

        public IForySerializer Serializer { get; }
    }
}
