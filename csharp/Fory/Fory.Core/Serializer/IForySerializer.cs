using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Encoding;
using Fory.Core.Spec;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core.Serializer;

public abstract class ForySerializerBase : IForySerializer
{
    public abstract Type AssociatedType { get; }

    public abstract Task SerializeDataAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    public abstract ValueTask<object?> DeserializeDataAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Fory header specification: | magic number | reserved bits |  oob  | xlang | endian | null  |  language  | unsigned
    ///     int for meta start offset |
    /// </summary>
    public async Task SerializeHeaderInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var flag = ForyHeaderSpec.ForyConfigFlags.IsLittleEdian;
        flag |= value is null ? ForyHeaderSpec.ForyConfigFlags.IsNull : flag;
        flag |= context.IsXlang ? ForyHeaderSpec.ForyConfigFlags.IsXlang : flag;

        var span = context.Writer.GetSpan(sizeof(ushort));
        if (context.IsXlang) BinaryPrimitives.WriteUInt16LittleEndian(span, ForyHeaderSpec.MagicNumber);

        context.Writer.Advance(sizeof(ushort));

        span = context.Writer.GetSpan(sizeof(byte));
        MemoryMarshal.Write(span, ref flag);
        context.Writer.Advance(sizeof(byte));

        if (context.IsXlang)
        {
            span = context.Writer.GetSpan(sizeof(byte));
            var code = ForyHeaderSpec.LanguageCode;
            MemoryMarshal.Write(span, ref code);
            context.Writer.Advance(sizeof(byte));
        }

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SerializeRefInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var refFlag =
            (byte)(value is null ? ForyRefMetaSpec.ReferenceFlag.Null : ForyRefMetaSpec.ReferenceFlag.NotNull);

        var span = context.Writer.GetSpan(sizeof(byte));
        MemoryMarshal.Write(span, ref refFlag);
        context.Writer.Advance(sizeof(byte));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SerializeTypeInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        if (value is null)
            return;

        var typeSpec = context.TypeSpecificationRegistry.GetTypeSpecification(value.GetType());
        switch (typeSpec)
        {
            case IKnownTypeSpecification knownTypeMetaSpec:
                var knownTypeSpan = context.Writer.GetSpan(sizeof(byte));
                var knownTypeIdEncoded = ForyEncoding.AsVarUInt32(knownTypeMetaSpec.GetTypeId()).First();
                MemoryMarshal.Write(knownTypeSpan, ref knownTypeIdEncoded);
                context.Writer.Advance(sizeof(byte));

                await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                return;
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

                await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                return;
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

                await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                return;
            case IExtTypeSpecification extTypeMetaSpec:
                var extTypeId = extTypeMetaSpec.GetTypeId();
                WriteForyTypeId(extTypeId);

                var extKnownType = ExtractKnownType(extTypeId);
                if (extKnownType == TypeSpecificationRegistry.KnownTypes.NamedExt)
                {
                    if (context.ShareMeta)
                        WriteTypeMetaIndex(extTypeMetaSpec);
                    else
                        WriteTypeMetaString(extTypeMetaSpec);
                }

                await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                return;
        }

        throw new NotSupportedException("Registered type meta unsupported.");

        void WriteForyTypeId(uint structTypeId)
        {
            var buffer = ForyEncoding.AsVarUInt32(structTypeId).ToArray();
            var bufferSpan = context.Writer.GetSpan(buffer.Length);
            buffer.CopyTo(bufferSpan);
            context.Writer.Advance(buffer.Length);
        }

        void WriteTypeMetaIndex<TTypeSpecification>(TTypeSpecification structTypeMetaSpec)
            where TTypeSpecification : IUserDefinedTypeSpecification
        {
            var metaIdx = context.TypeMetaRegistry.TryRegister(structTypeMetaSpec);
            var metaIdxEncoding = ForyEncoding.AsVarUInt32(metaIdx).ToArray();
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

    public async ValueTask<HeaderInfo> DeserializeHeaderInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var validMagicNumber = await ValidateMagicNumber(context, cancellationToken).ConfigureAwait(false);
        if (!validMagicNumber)
            throw new SerializationException(
                $"Fory xlang serialization must start with magic number {ForyHeaderSpec.MagicNumber}.");

        var readResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var sequence = readResult.Buffer.Slice(0, sizeof(byte));
        var bitmap = MemoryMarshal.Read<ForyHeaderSpec.ForyConfigFlags>(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        if (bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsXlang))
        {
            readResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            sequence = readResult.Buffer.Slice(0, sizeof(byte));
            var sourceLanguageCode = MemoryMarshal.Read<byte>(sequence.First.Span);
            context.Reader.AdvanceTo(sequence.End);

            return new HeaderInfo(bitmap, sourceLanguageCode);
        }

        return new HeaderInfo(bitmap);

        static async ValueTask<bool> ValidateMagicNumber(DeserializationContext context,
            CancellationToken cancellationToken)
        {
            if (!context.IsXlang)
                return true;

            var readResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var sequence = readResult.Buffer.Slice(0, sizeof(ushort));

            var pool = ArrayPool<byte>.Shared;
            var buffer = pool.Rent(sizeof(ushort));
            foreach (var seq in sequence) seq.Span.CopyTo(buffer);

            var magicNumber = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(0, sizeof(ushort)));
            pool.Return(buffer);
            context.Reader.AdvanceTo(sequence.End);

            return magicNumber == ForyHeaderSpec.MagicNumber;
        }
    }

    public async ValueTask<ReferenceInfo> DeserializeRefInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var readResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var sequence = readResult.Buffer.Slice(0, sizeof(byte));
        var bitmap = MemoryMarshal.Read<ForyRefMetaSpec.ReferenceFlag>(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        return new ReferenceInfo(bitmap);
    }

    public async ValueTask<TypeInfo<TValue>> DeserializeTypeInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var foryTypeId = await ForyEncoding.FromVarUInt32Async(context.Reader, cancellationToken)
            .ConfigureAwait(false);

        switch (ExtractKnownType(foryTypeId))
        {
            case TypeSpecificationRegistry.KnownTypes.NamedCompatibleStruct
                or TypeSpecificationRegistry.KnownTypes.CompatibleStruct:

                break;
            case TypeSpecificationRegistry.KnownTypes.NamedStruct
                or TypeSpecificationRegistry.KnownTypes.NamedEnum
                or TypeSpecificationRegistry.KnownTypes.NamedExt:
                break;
        }

        context.TypeSpecificationRegistry.TryGetTypeSpecification(foryTypeId, out var typeSpec);
        return new TypeInfo<TValue>(typeSpec);
    }

    private static TypeSpecificationRegistry.KnownTypes ExtractKnownType(uint typeId)
    {
        // Reserved bits of the type id. Represents the fory data type.
        const byte knownTypeReservedBits = 0xff;
        return (TypeSpecificationRegistry.KnownTypes)(typeId & knownTypeReservedBits);
    }
}

public abstract class ForySerializerBase<TValue> : ForySerializerBase, IForySerializer<TValue>
{
    public override Type AssociatedType => typeof(TValue);

    public abstract Task SerializeDataAsync(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    public abstract ValueTask<TValue> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default);

    public override Task SerializeDataAsync<TScopedValue>(TScopedValue value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        if (value is TValue casted) return SerializeDataAsync(casted, context, cancellationToken);

        throw new SerializationException($"Unable to serialize {value} using {GetType().Name} serializer");
    }

    public override async ValueTask<object?> DeserializeDataAsync<TScopedValue>(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        if (typeof(TScopedValue) == AssociatedType)
        {
            var value = await DeserializeDataAsync(context, cancellationToken);
            return value;
        }

        throw new SerializationException($"Unable to serialize using {GetType().Name} serializer");
    }
}

public interface IForySerializer<TValue> : IForySerializer
{
    Task SerializeDataAsync(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask<TValue> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default);
}

public interface IForySerializer
{
    Type AssociatedType { get; }

    Task SerializeHeaderInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    Task SerializeRefInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    Task SerializeTypeInfoAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    Task SerializeDataAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask<HeaderInfo> DeserializeHeaderInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask<ReferenceInfo> DeserializeRefInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask<TypeInfo<TValue>> DeserializeTypeInfoAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask<object?> DeserializeDataAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default);
}
