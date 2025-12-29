using System;
using System.Runtime.Serialization;

namespace Fory.Core
{
    public abstract class ForySerializerBase<TValue> : IForySerializer
    {
        public Type AssociatedType => typeof(TValue);
        protected abstract void OnSerialize(TValue value, SerializationContext context);

        public void Serialize(object value, SerializationContext context)
        {
            if (value is TValue casted)
                OnSerialize(casted, context);

            throw new SerializationException($"Cannot serialize {AssociatedType.Name} using {GetType().Name} serializer");
        }
    }

    public interface IForySerializer
    {
        Type AssociatedType { get; }

        void Serialize(object value, SerializationContext context);
    }
}
