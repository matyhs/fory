using System;
using System.Collections.Generic;
using Fory.Core.Spec.DataType;

namespace Fory.Core.Spec.Meta
{
    internal class TypeMetaRegistry
    {
        private readonly Dictionary<Type, uint> _typeIndexMap;
        private readonly Queue<byte[]> _typeDefinitions;
        private readonly TypeSpecificationRegistry _typeSpecificationRegistry;

        internal TypeMetaRegistry(TypeSpecificationRegistry typeSpecificationRegistry)
        {
            _typeDefinitions = new Queue<byte[]>();
            _typeIndexMap = new Dictionary<Type, uint>();

            _typeSpecificationRegistry = typeSpecificationRegistry;
        }

        public uint TryRegister(ITypeSpecification typeSpecification)
        {
            var type = typeSpecification.AssociatedType;
            if (_typeIndexMap.TryGetValue(type, out var index))
                return index;

            _typeDefinitions.Enqueue(TypeMetaResolver.Encode(typeSpecification, _typeSpecificationRegistry));

            index = (uint)_typeIndexMap.Count;
            _typeIndexMap.Add(type, index);
            return index;
        }
    }
}
