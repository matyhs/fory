using System;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType;

/// <summary>
///     Defines the type specification for a data type
/// </summary>
internal interface ITypeSpecification
{
    Type AssociatedType { get; }

    uint TypeId { get; }

    IForySerializer Serializer { get; }
}

internal interface ITypeSpecification<TType> : ITypeSpecification
{
    new IForySerializer<TType> Serializer { get; }
}

/// <summary>
///     Defines the type specification for system-defined data type
/// </summary>
internal interface IKnownTypeSpecification : ITypeSpecification
{
    TypeSpecificationRegistry.KnownTypes KnownTypeId { get; }
}

internal interface IKnownTypeSpecification<TType> : IKnownTypeSpecification, ITypeSpecification<TType>
{
}

/// <summary>
///     Defines user-defined type specification
/// </summary>
internal interface IUserDefinedTypeSpecification : ITypeSpecification
{
    bool IsRegisteredByName { get; }

    bool IsNamespaceIncluded { get; }
}

/// <summary>
///     Defines the type specification for enum data type
/// </summary>
internal interface IEnumTypeSpecification : IUserDefinedTypeSpecification
{
}

/// <summary>
///     Defines the type specification for POCO
/// </summary>
internal interface IStructTypeSpecification : IUserDefinedTypeSpecification
{
}

/// <summary>
///     Defines the type specification for POCO with custom serializer
/// </summary>
internal interface IExtTypeSpecification : IUserDefinedTypeSpecification
{
    Type SerializerType { get; }
}
