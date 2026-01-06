using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Serializer;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core
{
    public class Fory
    {
        private readonly ForyOptions _options;
        private readonly TypeSpecificationRegistry _typeSpecificationRegistry = new TypeSpecificationRegistry();

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

        public async ValueTask<ReadOnlySequence<byte>> SerializeAsync<TValue>(TValue value, CancellationToken cancellationToken = default)
        {
            var context = new SerializationContext(_options, _typeSpecificationRegistry);
            var typeSpec = _typeSpecificationRegistry.GetTypeSpecification(typeof(TValue));

            await typeSpec.Serializer.SerializeHeaderInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
            await typeSpec.Serializer.SerializeRefInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
            await typeSpec.Serializer.SerializeTypeInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
            await typeSpec.Serializer.SerializeDataAsync(value, context, cancellationToken).ConfigureAwait(false);

            return await context.CompleteAsync(cancellationToken);
        }

        public ValueTask<TValue> DeserializeAsync<TValue>(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken = default)
            where TValue : new()
        {
            var context = new DeserializationContext(_options, _typeSpecificationRegistry);
            context.Initialize(buffer);

            var typeSpec = _typeSpecificationRegistry.GetTypeSpecification(typeof(TValue));
            typeSpec.Serializer.DeserializeHeaderInfoAsync<TValue>(context, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
