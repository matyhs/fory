using System;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType
{
    internal class ExtTypeSpecification<TSerializer> : IExtTypeSpecification
        where TSerializer : IForySerializer, new()
    {
        private readonly Lazy<TSerializer> _serializer;

        public Type SerializerType => typeof(TSerializer);
        public Type AssociatedType => _serializer.Value.AssociatedType;
        public uint TypeId { get; }
        public bool IsRegisteredByName { get; }
        public bool IsNamespaceIncluded { get; }
        public IForySerializer Serializer => _serializer.Value;

        public ExtTypeSpecification(bool includeNamespace) : this()
        {
            IsRegisteredByName = true;
            IsNamespaceIncluded = includeNamespace;
        }

        public ExtTypeSpecification(uint typeId) : this()
        {
            TypeId = typeId;
            IsRegisteredByName = false;
        }

        private ExtTypeSpecification()
        {
            _serializer = new Lazy<TSerializer>(() => new TSerializer());
        }
    }
}
