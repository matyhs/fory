// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Encoding;

namespace Fory.Core.Serializer;

internal sealed class EnumSerializer : ForySerializerBase
{
    internal static readonly Lazy<IForySerializer> Instance = new(() => new EnumSerializer());

    public override Type AssociatedType => typeof(Enum);

    public override async Task SerializeDataAsync<TValue>(TValue value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        if (!value.GetType().IsEnum)
            throw new SerializationException($"Unable to serialize {value} using enum serializer");

        var underlyingValue = GetUnderlyingValueAsUInt32(value);
        var converted = ForyEncoding.AsVarUInt32(underlyingValue).ToArray();
        var span = context.Writer.GetSpan(converted.Length);
        converted.CopyTo(span);
        context.Writer.Advance(converted.Length);

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override ValueTask<object?> DeserializeDataAsync<TValue>(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static uint GetUnderlyingValueAsUInt32<TValue>(TValue value)
    {
        switch (value)
        {
            case sbyte s:
                return (uint)s;
            case byte b:
                return b;
            case short sh:
                return (uint)sh;
            case ushort ush:
                return ush;
            case int i:
                return (uint)i;
            case uint ui:
                return ui;
            case IntPtr intPtr:
                return (uint)intPtr;
            case UIntPtr uIntPtr:
                return (uint)uIntPtr;
            default:
                throw new SerializationException(
                    $"The underlying integral numeric type for {value.GetType()} is not supported. Create a custom serializer to support the serialization of this enum.");
        }
    }
}
