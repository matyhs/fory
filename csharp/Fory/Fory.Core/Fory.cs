using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Extensions;
using Fory.Core.Serializer;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core;

public class Fory
{
    private readonly ForyOptions _options;
    private readonly TypeSpecificationRegistry _typeSpecificationRegistry = new();

    public Fory() : this(ForyOptions.Default)
    {
    }

    public Fory(ForyOptions options)
    {
        _options = options;
    }

    public bool Register<TObject>()
    {
        throw new NotImplementedException();
    }

    public bool RegisterSerializer<TSerializer>() where TSerializer : IForySerializer
    {
        throw new NotImplementedException();
    }

    public async ValueTask<ReadOnlySequence<byte>> SerializeAsync<TValue>(TValue value,
        CancellationToken cancellationToken = default)
    {
        var context = new SerializationContext(_options, _typeSpecificationRegistry);
        var typeSpec = _typeSpecificationRegistry.GetTypeSpecification(typeof(TValue));

        await typeSpec.Serializer.SerializeHeaderInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
        if (value is not null)
        {
            await typeSpec.Serializer.SerializeRefInfoAsync(value, context, cancellationToken)
                .ConfigureAwait(false);
            await typeSpec.Serializer.SerializeTypeInfoAsync(value, context, cancellationToken)
                .ConfigureAwait(false);
            await typeSpec.Serializer.SerializeDataAsync(value, context, cancellationToken).ConfigureAwait(false);
        }

        return await context.CompleteAsync(cancellationToken);
    }

    public async ValueTask<TValue?> DeserializeAsync<TValue>(ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        var context = new DeserializationContext(_options, _typeSpecificationRegistry);
        context.Initialize(buffer);

        var typeSpec = _typeSpecificationRegistry.GetTypeSpecification(typeof(TValue));
        var headerInfo = await typeSpec.Serializer.DeserializeHeaderInfoAsync<TValue>(context, cancellationToken)
            .ConfigureAwait(false);
        if (headerInfo.IsHeaderValidOrThrow(context) && headerInfo.IsNull)
            return default;

        var referenceInfo = await typeSpec.Serializer.DeserializeRefInfoAsync<TValue>(context, cancellationToken)
            .ConfigureAwait(false);
        if (referenceInfo.IsNull)
            return default;

        var typeInfo = await typeSpec.Serializer.DeserializeTypeInfoAsync<TValue>(context, cancellationToken)
            .ConfigureAwait(false);
        var stronglyTypedSerializer = typeInfo.GetTypedSerializer();
        if (stronglyTypedSerializer is not null)
            return await stronglyTypedSerializer.DeserializeDataAsync(context, cancellationToken)
                .ConfigureAwait(false);

        var value = await typeInfo.Serializer.DeserializeDataAsync<TValue>(context, cancellationToken);
        if (value is null)
            return default;

        return (TValue)value;
    }
}
