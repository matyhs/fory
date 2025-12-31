using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType
{
    public interface ITypeSerializer
    {
        IForySerializer Serializer { get; }
    }

    public interface ITypeSerializer<in TType> : ITypeSerializer
    {
        new IForySerializer<TType> Serializer { get; }
    }
}
