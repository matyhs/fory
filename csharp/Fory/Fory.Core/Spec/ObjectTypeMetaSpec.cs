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
                    var enumTypeId = enumTypeMetaSpec.GetTypeId();
                    WriteForyTypeId(enumTypeId);

                    var enumKnownType = ExtractKnownType(enumTypeId);
                    if (enumKnownType == TypeSpecificationRegistry.KnownTypes.NamedEnum)
                    {
                        if (context.ShareMeta)
                            WriteTypeMetaIndex(enumTypeMetaSpec);
                        else
                            WriteTypeMetaString(enumTypeMetaSpec);
                    }
                    break;
                case IStructTypeSpecification structTypeMetaSpec:
                    var structTypeId = structTypeMetaSpec.GetTypeId();
                    WriteForyTypeId(structTypeId);

                    var structKnownType = ExtractKnownType(structTypeId);
                    switch (structKnownType)
                    {
                        case TypeSpecificationRegistry.KnownTypes.NamedCompatibleStruct:
                        case TypeSpecificationRegistry.KnownTypes.CompatibleStruct:
                            WriteTypeMetaIndex(structTypeMetaSpec);
                            break;
                        case TypeSpecificationRegistry.KnownTypes.NamedStruct:
                            if (context.ShareMeta)
                                WriteTypeMetaIndex(structTypeMetaSpec);
                            else
                                WriteTypeMetaString(structTypeMetaSpec);
                            break;
                    }

                    return Task.CompletedTask;
            }

            throw new NotSupportedException("Registered type meta unsupported.");

            void WriteForyTypeId(uint structTypeId)
            {
                var buffer = ForyEncoding.AsVarInt32(structTypeId).ToArray();
                var bufferSpan = context.Writer.GetSpan(buffer.Length);
                buffer.CopyTo(bufferSpan);
                context.Writer.Advance(buffer.Length);
            }

            void WriteTypeMetaIndex<TTypeSpecification>(TTypeSpecification structTypeMetaSpec)
                where TTypeSpecification : IUserDefinedTypeSpecification
            {
                var metaIdx = context.TypeMetaRegistry.TryRegister(structTypeMetaSpec);
                var metaIdxEncoding = ForyEncoding.AsVarInt32(metaIdx).ToArray();
                var structSpan = context.Writer.GetSpan(metaIdxEncoding.Length);
                metaIdxEncoding.CopyTo(structSpan);
                context.Writer.Advance(metaIdxEncoding.Length);
            }

            void WriteTypeMetaString<TTypeSpecification>(TTypeSpecification structTypeMetaSpec)
                where TTypeSpecification : IUserDefinedTypeSpecification
            {
                var buffer = context.TypeMetaStringRegistry.TryRegister(structTypeMetaSpec);
                var span = context.Writer.GetSpan(buffer.Length);
                buffer.CopyTo(span);
                context.Writer.Advance(buffer.Length);
            }
        }

        private static TypeSpecificationRegistry.KnownTypes ExtractKnownType(uint typeId)
        {
            // Reserved bits of the type id. Represents the fory data type.
            const byte knownTypeReservedBits = 0xff;
            return (TypeSpecificationRegistry.KnownTypes) (typeId & knownTypeReservedBits);
        }
    }
}
