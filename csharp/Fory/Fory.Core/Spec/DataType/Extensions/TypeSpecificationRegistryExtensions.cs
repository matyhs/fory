using System;

namespace Fory.Core.Spec.DataType.Extensions
{
    internal static class TypeSpecificationRegistryExtensions
    {
        public static ITypeSpecification GetTypeSpecification(this TypeSpecificationRegistry registry, Type type)
        {
            return registry.GetTypeSpecification(type) ?? registry.GetTypeSpecification(type.GetGenericTypeDefinition());
        }
    }
}
