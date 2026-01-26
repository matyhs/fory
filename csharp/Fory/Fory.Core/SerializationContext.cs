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
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.Meta;

namespace Fory.Core;

public sealed class SerializationContext
{
    private readonly ForyOptions _options;
    private readonly Pipe _pipe;

    private int _startMetaOffset = -1;
    private int _metaOffset = -1;

    internal SerializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _pipe = new Pipe();
        TypeMetaRegistry = new TypeMetaRegistry(typeSpecificationRegistry);
        TypeMetaStringRegistry = new TypeMetaStringRegistry();

        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeWriter Writer => _pipe.Writer;

    public bool IsXlang => _options.Xlang;

    public bool ShareMeta => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }
    internal TypeMetaRegistry TypeMetaRegistry { get; private set; }
    internal TypeMetaStringRegistry TypeMetaStringRegistry { get; private set; }

    internal void InitializeStartMetaOffset()
    {
        if (!_options.Compatible) return;

        _startMetaOffset = (int) _pipe.Writer.UnflushedBytes;

        var placeholder = -1;
        var span = _pipe.Writer.GetSpan(sizeof(int));
        MemoryMarshal.Write(span, ref placeholder);
        _pipe.Writer.Advance(sizeof(int));
    }

    internal async Task CalculateMetaOffsetAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Validate that type meta registry also has entries
        if (!_options.Compatible || !TypeMetaRegistry.HasEntry()) return;

        var readResult = await _pipe.Reader.ReadAsync(cancellationToken);
        // total written bytes - header bytes - placeholder meta offset bytes
        _metaOffset = (int)(readResult.Buffer.Length - _startMetaOffset - sizeof(int));
    }

    internal async ValueTask<ReadOnlySequence<byte>> CompleteAsync(CancellationToken cancellationToken = default)
    {
        await Writer.CompleteAsync().ConfigureAwait(false);
        if (!_pipe.Reader.TryRead(out var readResult))
            throw new SerializationException("Error occurred while accessing buffer data");

        if (!_options.Compatible)
            return readResult.Buffer;

        // Note: the buffer is immutable after writing via PipeWriter.
        var headerSequence = readResult.Buffer.Slice(0, _startMetaOffset);
        var metaOffsetMemory = BitConverter.GetBytes(_metaOffset).AsMemory();
        var remainingSequence = readResult.Buffer.Slice(_startMetaOffset + sizeof(int));

        var firstSegment = new ZeroCopyByteSegment(headerSequence.First);
        var lastSegment = firstSegment;

        var remainingHeader = headerSequence.Slice(headerSequence.First.Length);
        if (!remainingHeader.IsEmpty)
        {
            foreach (var memory in remainingHeader)
            {
                lastSegment = lastSegment.Append(memory);
            }
        }

        lastSegment = lastSegment.Append(metaOffsetMemory);

        if (!remainingSequence.IsEmpty)
        {
            foreach (var memory in remainingSequence)
            {
                lastSegment = lastSegment.Append(memory);
            }
        }

        return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
    }

    private class ZeroCopyByteSegment : ReadOnlySequenceSegment<byte>
    {
        public ZeroCopyByteSegment(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public ZeroCopyByteSegment Append(ReadOnlyMemory<byte> memory)
        {
            var next = new ZeroCopyByteSegment(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = next;
            return next;
        }
    }
}

public sealed class DeserializationContext
{
    private readonly ForyOptions _options;
    private readonly Pipe _pipe;

    internal DeserializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _pipe = new Pipe();

        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeReader Reader => _pipe.Reader;

    public bool IsXlang => _options.Xlang;

    public bool IsCompatible => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

    internal void Initialize(ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsSingleSegment)
            _pipe.Writer.Write(buffer.First.Span);
        else
            foreach (var segment in buffer)
                _pipe.Writer.Write(segment.Span);

        _pipe.Writer.Complete();
    }
}
