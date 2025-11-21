using System;
using System.Linq;
using System.Threading.Tasks;
using Fory.Core.Encoding;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core.Spec
{
    internal class ObjectTypeMetaSpec : IForySpecDefinition
    {
        /// <summary>
        /// Reserved bits of the type id. Represents the fory data type.
        /// </summary>
        private const byte KNOWN_TYPE_RESERVED_BITS = 0xff;

        public Task Serialize<TValue>(in TValue value, SerializationContext context)
        {
            if (value is null)
                return Task.CompletedTask;

            var typeMeta = context.TypeSpecificationRegistry[value.GetType()];

            switch (typeMeta)
            {
                case IKnownTypeSpecification knownTypeMetaSpec:
                    var knownTypeSpan = context.Writer.GetSpan(1);
                    var knownTypeIdEncoded = ForyEncoding.AsVarInt32(knownTypeMetaSpec.GetTypeId()).First();
                    knownTypeSpan.Fill(knownTypeIdEncoded);
                    context.Writer.Advance(1);
                    return Task.CompletedTask;
                case IEnumTypeSpecification enumTypeMetaSpec:

                    break;
                case IStructTypeSpecification structTypeMetaSpec:
                    var structTypeId = structTypeMetaSpec.GetTypeId();
                    var structTypeIdEncoded = ForyEncoding.AsVarInt32(structTypeId).ToArray();
                    var structTypeSpan = context.Writer.GetSpan(structTypeIdEncoded.Length);
                    structTypeIdEncoded.CopyTo(structTypeSpan);

                    var knownType = ExtractKnownType(structTypeId);
                    switch (knownType)
                    {
                        case TypeSpecificationRegistry.KnownTypes.NamedCompatibleStruct:
                        case TypeSpecificationRegistry.KnownTypes.CompatibleStruct:

                            break;
                        case TypeSpecificationRegistry.KnownTypes.NamedStruct:
                            if (context.ShareMeta)
                            {

                            }
                            break;
                    }

                    return Task.CompletedTask;
            }

            throw new NotSupportedException("Registered type meta unsupported.");
        }

        private static TypeSpecificationRegistry.KnownTypes ExtractKnownType(uint typeId)
        {
            return (TypeSpecificationRegistry.KnownTypes) (typeId & KNOWN_TYPE_RESERVED_BITS);
        }
    }
}
