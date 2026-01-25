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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Encoding;

namespace Fory.Core.Serializer;

internal sealed class DurationSerializer : ForySerializerBase<TimeSpan>
{
    public override async Task SerializeDataAsync(TimeSpan value, SerializationContext context,
        CancellationToken cancellationToken = default)
    {
        var secondsPart = (long)Math.Truncate(value.TotalSeconds);
        var nanosecondsPart = (int)(value.Ticks * 100 - secondsPart * 1_000_000_000);
        var secondsPartBuffer = ForyEncoding.AsVarInt64(secondsPart).ToArray();

        var span = context.Writer.GetSpan(secondsPartBuffer.Length);
        secondsPartBuffer.CopyTo(span);
        context.Writer.Advance(secondsPartBuffer.Length);

        span = context.Writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, nanosecondsPart);
        context.Writer.Advance(sizeof(int));

        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<TimeSpan> DeserializeDataAsync(DeserializationContext context,
        CancellationToken cancellationToken = default)
    {
        var secondsPart = await ForyEncoding.FromVarInt64Async(context.Reader, cancellationToken).ConfigureAwait(false);
        var readerResult = await context.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var sequence = readerResult.Buffer.Slice(0, sizeof(int));
        var nanosecondsPart = BinaryPrimitives.ReadInt32LittleEndian(sequence.First.Span);
        context.Reader.AdvanceTo(sequence.End);

        var ticks = secondsPart * 10_000_000 + nanosecondsPart / 100;
        return TimeSpan.FromTicks(ticks);
    }
}
