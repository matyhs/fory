namespace Fory.Core.Spec.DataType.Extensions
{
    internal static class TypeSpecificationExtensions
    {
        internal static ushort GetTypeId(this IKnownTypeSpecification typeSpec)
        {
            return (ushort) typeSpec.TypeId;
        }

        internal static uint GetTypeId(this IEnumTypeSpecification typeSpec)
        {
            return typeSpec.IsRegisteredByName
                ? (uint) TypeSpecificationRegistry.KnownTypes.NamedEnum
                : (typeSpec.TypeId << 8) + (uint) TypeSpecificationRegistry.KnownTypes.Enum; // Shift left to reserve the first 8 bits to represent the fory type id
        }

        internal static uint GetTypeId(this IStructTypeSpecification typeSpec, bool compatible = false)
        {
            if (compatible)
            {
                return typeSpec.IsRegisteredByName
                    ? (uint)TypeSpecificationRegistry.KnownTypes.NamedCompatibleStruct
                    : (typeSpec.TypeId << 8) + (uint)TypeSpecificationRegistry.KnownTypes.CompatibleStruct;
            }

            if (typeSpec.IsRegisteredByName)
            {
                return (uint)TypeSpecificationRegistry.KnownTypes.NamedStruct;
            }

            return (typeSpec.TypeId << 8) + (uint)TypeSpecificationRegistry.KnownTypes.Struct;
        }

        internal static string GetNamespace(this IUserDefinedTypeSpecification typeSpec)
        {
            return typeSpec.IsNamespaceIncluded ? typeSpec.AssociatedType.Namespace ?? string.Empty : string.Empty;
        }
    }
}
