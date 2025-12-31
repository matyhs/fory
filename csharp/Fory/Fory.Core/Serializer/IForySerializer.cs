using System;
using System.Runtime.Serialization;

namespace Fory.Core.Serializer
{
    public abstract class ForySerializerBase<TValue> : IForySerializer<TValue>
    {
        public Type AssociatedType => typeof(TValue);

        public abstract void Serialize(TValue value, SerializationContext context);

        public void Serialize<TScopedValue>(TScopedValue value, SerializationContext context)
        {
            if (value is TValue casted)
                Serialize(casted, context);

            throw new SerializationException($"Unable to serialize {value} using {GetType().Name} serializer");
        }
    }

    public interface IForySerializer<in TValue> : IForySerializer
    {
        void Serialize(TValue value, SerializationContext context);
    }

    public interface IForySerializer
    {
        Type AssociatedType { get; }

        void Serialize<TValue>(TValue value, SerializationContext context);
    }
}
