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
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Utils;

namespace Fory.Core.Serializer;

internal sealed class LocalDateSerializer : ForySerializerBase<DateTime>
{
    public override async Task SerializeDataAsync(DateTime value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var days = (int)((value - DateTimeUtils.GetUnixEpoch().Date).Ticks / TimeSpan.TicksPerDay);
        var span = context.Writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, days);
        context.Writer.Advance(sizeof(int));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<DateTime> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var readResult = await context.Reader.ReadAsync(cancellationToken);
        var sequence = readResult.Buffer.Slice(0, sizeof(int));
        var value = BinaryPrimitives.ReadInt32LittleEndian(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        return DateTimeUtils.GetUnixEpoch().DateTime.AddDays(value);
    }
}
