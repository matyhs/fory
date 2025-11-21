using System;
using System.Collections.Generic;
using Fory.Core.Spec.DataType;

namespace Fory.Core.Spec.Meta
{
    internal class TypeMetaRegistry
    {
        private readonly Dictionary<Type, uint> _registry;

        internal TypeMetaRegistry()
        {
            _registry = new Dictionary<Type, uint>();
        }

        public uint TryRegister(ITypeSpecification typeSpecification)
        {
            var type = typeSpecification.AssociatedType;
            if (_registry.TryGetValue(type, out var index))
                return index;

            index = (uint) _registry.Count;
            _registry.Add(type, index);
            return index;
        }
    }
}
