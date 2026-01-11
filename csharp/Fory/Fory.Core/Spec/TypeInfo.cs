using Fory.Core.Serializer;
using Fory.Core.Spec.DataType;

namespace Fory.Core.Spec;

public class TypeInfo<TValue>
{
    private readonly ITypeSpecification<TValue>? _stronglyTypedSpecification;
    private readonly ITypeSpecification _typeSpecification;

    internal TypeInfo(ITypeSpecification typeSpecification)
    {
        _typeSpecification = typeSpecification;
        _stronglyTypedSpecification = typeSpecification as ITypeSpecification<TValue>;
    }

    public IForySerializer Serializer => _typeSpecification.Serializer;

    public IForySerializer<TValue>? GetTypedSerializer()
    {
        return _stronglyTypedSpecification?.Serializer ?? null;
    }
}
