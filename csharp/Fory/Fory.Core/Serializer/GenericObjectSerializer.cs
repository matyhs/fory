using System;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core.Serializer
{
    /// <summary>
    /// Default object serializer. Serializes all properties of the object using reflection.
    /// </summary>
    public class GenericObjectSerializer : IForySerializer
    {
        public Type AssociatedType => typeof(object);

        public void Serialize<TValue>(TValue value, SerializationContext context)
        {
            var typeSpec = context.TypeSpecificationRegistry.GetTypeSpecification(value.GetType());
            var properties = typeSpec.AssociatedType.GetProperties();
            foreach (var property in properties)
            {

            }
        }
    }
}
