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
using System.Linq;
using System.Runtime.InteropServices;
using Fory.Core.Encoding;

namespace Fory.Core.Spec.Meta;

internal static class TypeMetaStringResolver
{
    /// <summary>
    ///     Meta string encoding with deduplication
    /// </summary>
    /// <param name="index"></param>
    /// <param name="metaStringBytes"></param>
    /// <returns></returns>
    public static byte[] Encode(uint index, MetaStringBytes? metaStringBytes)
    {
        if (metaStringBytes is null)
            return EncodeReference(index);

        return EncodeFirstOccurence(metaStringBytes);
    }

    /// <summary>
    ///     | ((id + 1) shift left 1) | 1 |
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private static byte[] EncodeReference(uint index)
    {
        index = ((index + 1) << 1) | 1;
        return ForyEncoding.AsVarUInt32(index).ToArray();
    }

    /// <summary>
    ///     | (length shift left 1) | [hash if large] | encoding | bytes |
    /// </summary>
    /// <param name="metaStringBytes"></param>
    /// <returns></returns>
    private static byte[] EncodeFirstOccurence(MetaStringBytes metaStringBytes)
    {
        const byte smallStringThreshold = 16;
        var length = metaStringBytes.Bytes.Length;
        var buffer = ForyEncoding.AsVarUInt32((uint)length << 1).ToArray();
        var writtenBytes = buffer.Length;
        if (length > smallStringThreshold)
        {
            Array.Resize(ref buffer, buffer.Length + length + 8);
            var hashCode = metaStringBytes.HashCode;
            var span = buffer.AsSpan(writtenBytes, 8);
            MemoryMarshal.Write(span, ref hashCode);
            writtenBytes += 8;
        }
        else
        {
            Array.Resize(ref buffer, buffer.Length + length + 1);
            var span = buffer.AsSpan(writtenBytes, 1);
            span.Fill(metaStringBytes.Encoding);
            writtenBytes += 1;
        }

        metaStringBytes.Bytes.CopyTo(buffer, writtenBytes);

        return buffer;
    }
}
