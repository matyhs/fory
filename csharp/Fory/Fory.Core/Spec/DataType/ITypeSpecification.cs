using System;
using System.Threading.Tasks;

namespace Fory.Core.Spec.DataType
{
    /// <summary>
    /// Defines the type specification for a data type
    /// </summary>
    internal interface ITypeSpecification
    {
        Type AssociatedType { get; }

        uint TypeId { get; }

        Task Serialize(object value, SerializationContext context);
    }

    internal interface ITypeSpecification<in TType> : ITypeSpecification
    {
        new Task Serialize(TType value, SerializationContext context);
    }

    /// <summary>
    /// Defines the type specification for system-defined data type
    /// </summary>
    internal interface IKnownTypeSpecification : ITypeSpecification, ITypeSerializer
    {
        TypeSpecificationRegistry.KnownTypes KnownTypeId { get; }
    }

    internal interface IKnownTypeSpecification<in TType> : IKnownTypeSpecification, ITypeSpecification<TType>, ITypeSerializer<TType>
    {

    }

    /// <summary>
    /// Defines user-defined type specification
    /// </summary>
    internal interface IUserDefinedTypeSpecification : ITypeSpecification, ITypeSerializer
    {
        bool IsRegisteredByName { get; }

        bool IsNamespaceIncluded { get; }
    }

    /// <summary>
    /// Defines the type specification for enum data type
    /// </summary>
    internal interface IEnumTypeSpecification : IUserDefinedTypeSpecification
    {

    }

    /// <summary>
    /// Defines the type specification for POCO
    /// </summary>
    internal interface IStructTypeSpecification : IUserDefinedTypeSpecification
    {

    }

    /// <summary>
    /// Defines the type specification for POCO with custom serializer
    /// </summary>
    internal interface IExtTypeSpecification : IUserDefinedTypeSpecification
    {
        Type SerializerType { get; }
    }
}
