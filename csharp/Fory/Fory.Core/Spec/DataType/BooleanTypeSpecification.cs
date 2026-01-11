using System;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType;

internal class BooleanTypeSpecification : IKnownTypeSpecification<bool>
{
    private readonly Lazy<IForySerializer<bool>> _serializer = new(() => new BooleanSerializer());

    public Type AssociatedType => typeof(bool);
    public uint TypeId => (uint)KnownTypeId;
    public TypeSpecificationRegistry.KnownTypes KnownTypeId => TypeSpecificationRegistry.KnownTypes.Boolean;
    IForySerializer ITypeSpecification.Serializer => Serializer;
    public IForySerializer<bool> Serializer => _serializer.Value;
}
