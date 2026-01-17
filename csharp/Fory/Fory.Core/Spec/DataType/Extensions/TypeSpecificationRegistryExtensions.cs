using System;

namespace Fory.Core.Spec.DataType.Extensions;

internal static class TypeSpecificationRegistryExtensions
{
    /// <summary>
    ///     Get the registered type specification. This extension method allows a fallback type specification retrieval in the
    ///     case of generic types registration.
    /// </summary>
    /// <param name="registry">type specification registry</param>
    /// <param name="type">type to locate</param>
    /// <returns>returns the type specification of the given type</returns>
    /// <exception cref="NotSupportedException">thrown when type is not registered</exception>
    public static ITypeSpecification GetTypeSpecification(this TypeSpecificationRegistry registry, Type type)
    {
        var exception = new NotSupportedException($"Type is not registered: {type.FullName}");
        return registry.TryGetTypeSpecification(type, out var typeSpec)
            ? typeSpec
            : type.ContainsGenericParameters
                ? registry.TryGetTypeSpecification(type.GetGenericTypeDefinition(), out typeSpec)
                    ? typeSpec
                    : throw exception
                : throw exception;
    }
}
