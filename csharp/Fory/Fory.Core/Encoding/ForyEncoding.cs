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
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fory.Core.Encoding;

internal static class ForyEncoding
{
    /// <summary>
    ///     Fory VarUInt32, consisting of 1~5 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
    ///     most significant bit (MSB) indicates the existence of another byte.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<byte> AsVarUInt32(uint value)
    {
        for (var i = 0; i < 5; i++)
        {
            if (value < 0x80)
            {
                yield return (byte)value;
                yield break;
            }

            yield return (byte)((value & 0x7f) | 0x80);
            value >>= 7;
        }
    }

    /// <summary>
    ///     Fory VarUInt32, consisting of 1~5 bytes, decodes buffer by reading a 7-bit grouping per byte. The
    ///     most significant bit (MSB) is disregarded from the decoded value.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<uint> FromVarUInt32Async(PipeReader reader,
        CancellationToken cancellationToken = default)
    {
        uint value = 0;
        for (var i = 0; i < 5; i++)
        {
            var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var sequence = readResult.Buffer.Slice(0, sizeof(byte));
            var local = MemoryMarshal.Read<byte>(sequence.First.Span);
            value |= (uint)(local & 0x7f) << (7 * i);
            reader.AdvanceTo(sequence.End);

            if (local < 0x80)
                break;
        }

        return value;
    }

    /// <summary>
    ///     Fory VarUInt64, consisting of 1~9 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
    ///     most significant bit (MSB) indicates the existence of another byte.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<byte> AsVarUInt64(ulong value)
    {
        for (var i = 0; i < 9; i++)
        {
            if (value < 0x80)
            {
                yield return (byte)value;
                yield break;
            }

            yield return (byte)((value & 0x7f) | 0x80);
            value >>= 7;
        }
    }

    /// <summary>
    ///     Fory VarUInt64, consisting of 1~9 bytes, decodes buffer by reading a 7-bit grouping per byte. The
    ///     most significant bit (MSB) is disregarded from the decoded value.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<ulong> FromVarUInt64Async(PipeReader reader,
        CancellationToken cancellationToken = default)
    {
        ulong value = 0;
        for (var i = 0; i < 9; i++)
        {
            var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var sequence = readResult.Buffer.Slice(0, sizeof(byte));
            var local = MemoryMarshal.Read<byte>(sequence.First.Span);
            value |= (ulong)(local & 0x7f) << (7 * i);
            reader.AdvanceTo(sequence.End);

            if (local < 0x80)
                break;
        }

        return value;
    }

    /// <summary>
    ///     Fory VarUInt36, consisting of 1~6 bytes, encodes unsigned integers to fit in a 7-bit grouping per byte. The
    ///     most significant bit (MSB) indicates the existence of another byte.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<byte> AsVarUInt36Small(ulong value)
    {
        const ulong maxValue = 1ul << 36;
        if (value >= maxValue)
            throw new NotSupportedException("Value too large for 36-bit encoding");

        return AsVarUInt64(value);
    }

    /// <summary>
    ///     Fory VarUInt36, consisting of 1~6 bytes, decodes buffer by reading a 7-bit grouping per byte. The
    ///     most significant bit (MSB) is disregarded from the decoded value.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OverflowException">Occurs when the decoded value exceed 36-bits</exception>
    public static async ValueTask<ulong> FromVarUInt36Async(PipeReader reader,
        CancellationToken cancellationToken = default)
    {
        ulong value = 0;
        for (var i = 0; i < 6; i++)
        {
            var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var sequence = readResult.Buffer.Slice(0, sizeof(byte));
            var local = MemoryMarshal.Read<byte>(sequence.First.Span);
            value |= (ulong)(local & 0x7f) << (7 * i);
            reader.AdvanceTo(sequence.End);

            if (local < 0x80)
                break;

            if (i == 5)
                throw new OverflowException("Value too large for 36-bit encoding");
        }

        return value;
    }

    /// <summary>
    ///     Fory VarInt64, consisting of 1~9 bytes, encodes signed integers to fit in a 7-bit grouping per byte. The
    ///     signed integer is first mapped into positive unsigned integer using ZigZag encoding, then encoded as unsigned
    ///     varint64
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<byte> AsVarInt64(long value)
    {
        var zigzag = (ulong)((value << 1) ^ (value >> 63));
        return AsVarUInt64(zigzag);
    }

    /// <summary>
    ///     Fory VarUInt64, consisting of 1~9 bytes, decodes buffer by reading a 7-bit grouping per byte. The value is
    ///     first decoded as varuint64, which represents the zigzag encoded value. This value is then decoded using
    ///     ZigZag encoding.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<long> FromVarInt64Async(PipeReader reader,
        CancellationToken cancellationToken = default)
    {
        var value = await FromVarUInt64Async(reader, cancellationToken).ConfigureAwait(false);
        return (long)(value >> 1) ^ -(long)(value & 1);
    }
}
