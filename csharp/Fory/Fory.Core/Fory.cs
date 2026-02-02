/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Buffers;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Extensions;
using Fory.Core.Serializer;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;
using Fory.Core.Spec.Ref;

namespace Fory.Core;

public class Fory
{
    private static readonly TypeSpecificationRegistry TypeSpecificationRegistry = new();

    private static readonly ThreadLocal<ContextCache<SerializationContext>> LocalSerializationContext =
        new(() => new ContextCache<SerializationContext>());

    private static readonly ThreadLocal<ContextCache<DeserializationContext>> LocalDeserializationContext =
        new(() => new ContextCache<DeserializationContext>());

    private static long _instanceCount = 0;

    private readonly long _instanceId;
    private readonly Lazy<TypeSpecificationRegistry> _finalRegistry = new(() => TypeSpecificationRegistry.DeepClone());
    private readonly ForyOptions _options;

    public Fory() : this(ForyOptions.Default)
    {
    }

    public Fory(ForyOptions options)
    {
        _options = options;
        _instanceId = Interlocked.Increment(ref _instanceCount);
    }

    public bool Register<TObject>()
    {
        throw new NotImplementedException();
    }

    public bool RegisterSerializer<TSerializer>() where TSerializer : IForySerializer
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Stream> SerializeAsync<TValue>(TValue value,
        CancellationToken cancellationToken = default)
    {
        var context = LocalSerializationContext.Value?.GetOrCreate(_instanceId,
            () => new SerializationContext(_options, TypeSpecificationRegistry.DeepClone()));
        if (context is null)
            throw new SerializationException("Error occured while creating serialization context");

        context.Initialize();

        var typeSpec = TypeSpecificationRegistry.GetTypeSpecification(typeof(TValue));
        await typeSpec.Serializer.SerializeHeaderInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
        if (value is not null)
        {
            var refMode = _options.TrackReference ? RefMode.Tracking : RefMode.NullOnly;
            await typeSpec.Serializer.SerializeContentAsync(value, context, refMode, true, cancellationToken)
                .ConfigureAwait(false);
        }

        return await context.CompleteAsync(cancellationToken);
    }

    public async Task SerializeAsync<TValue>(TValue value, Stream stream,
        CancellationToken cancellationToken = default)
    {
        var context = LocalSerializationContext.Value?.GetOrCreate(_instanceId,
            () => new SerializationContext(_options, TypeSpecificationRegistry.DeepClone()));
        if (context is null)
            throw new SerializationException("Error occured while creating serialization context");

        context.Initialize(stream);

        var typeSpec = TypeSpecificationRegistry.GetTypeSpecification(typeof(TValue));
        await typeSpec.Serializer.SerializeHeaderInfoAsync(value, context, cancellationToken).ConfigureAwait(false);
        if (value is not null)
        {
            var refMode = _options.TrackReference ? RefMode.Tracking : RefMode.NullOnly;
            await typeSpec.Serializer.SerializeContentAsync(value, context, refMode, true, cancellationToken)
                .ConfigureAwait(false);
        }

        await context.Writer.CompleteAsync().ConfigureAwait(false);
    }

    public async ValueTask<TValue?> DeserializeAsync<TValue>(Stream stream,
        CancellationToken cancellationToken = default)
    {
        using IDisposable _ = stream;
        var context = LocalDeserializationContext.Value?.GetOrCreate(_instanceId,
            () => new DeserializationContext(_options, TypeSpecificationRegistry.DeepClone()));
        if (context is null)
            throw new SerializationException("Error occured while creating deserialization context");

        context.Initialize(stream);

        var typeSpec = TypeSpecificationRegistry.GetTypeSpecification(typeof(TValue));
        var headerInfo = await typeSpec.Serializer.DeserializeHeaderInfoAsync<TValue>(context, cancellationToken)
            .ConfigureAwait(false);
        if (headerInfo.IsHeaderValidOrThrow(context) && headerInfo.IsNull)
            return default;

        var refMode = _options.TrackReference ? RefMode.Tracking : RefMode.NullOnly;
        return await typeSpec.Serializer.DeserializeContentAsync<TValue>(context, refMode, true, cancellationToken)
            .ConfigureAwait(false);
    }
}
