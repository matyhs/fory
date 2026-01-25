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
using System.Runtime.InteropServices;

namespace Fory.Core.HashAlgorithm;

/// <summary>
///     Reference for 128-bit MurmurHash3 Algorithm:
///     https://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
/// </summary>
internal static class MurmurHash3Algorithm
{
    private const ulong C1 = 0x87c37b91114253d5;
    private const ulong C2 = 0x4cf5ad432745937f;

    public static (ulong, ulong) ComputeHash(ref ReadOnlySpan<byte> data, ulong seed)
    {
        const uint readSize = 16;
        var length = (ulong)data.Length;
        var count = (int)length / readSize;
        var h1 = seed;
        var h2 = seed;

        var result = MemoryMarshal.Cast<byte, ulong>(data);
        for (var i = 0; i < count; i++)
        {
            var k1 = result[i * 2];
            var k2 = result[i * 2 + 1];

            h1 ^= MixKey1(k1);
            h1 = h1.RotateLeft(27);
            h1 += h2;
            h1 *= 5;
            h1 += 0x52dce729;

            h2 ^= MixKey2(k2);
            h2 = h2.RotateLeft(31);
            h2 += h1;
            h2 *= 5;
            h2 += 0x38495ab5;
        }

        h2 ^= ProcessRemainderKey2(ref data);
        h1 ^= ProcessRemainderKey1(ref data);

        h1 ^= length;
        h2 ^= length;

        h1 += h2;
        h2 += h1;

        h1 = MixFinal(h1);
        h2 = MixFinal(h2);

        h1 += h2;
        h2 += h1;

        return (h1, h2);

        ulong ProcessRemainderKey1(ref ReadOnlySpan<byte> local)
        {
            ulong k1 = 0;
            var lastIndex = (int)(count * readSize);
            if ((length & 15) >= 8)
                k1 ^= (ulong)local[lastIndex + 7] << 56;
            if ((length & 15) >= 7)
                k1 ^= (ulong)local[lastIndex + 6] << 48;
            if ((length & 15) >= 6)
                k1 ^= (ulong)local[lastIndex + 5] << 40;
            if ((length & 15) >= 5)
                k1 ^= (ulong)local[lastIndex + 4] << 32;
            if ((length & 15) >= 4)
                k1 ^= (ulong)local[lastIndex + 3] << 24;
            if ((length & 15) >= 3)
                k1 ^= (ulong)local[lastIndex + 2] << 16;
            if ((length & 15) >= 2)
                k1 ^= (ulong)local[lastIndex + 1] << 8;
            if ((length & 15) >= 1)
            {
                k1 ^= local[lastIndex];
                k1 = MixKey1(k1);
            }

            return k1;
        }

        ulong ProcessRemainderKey2(ref ReadOnlySpan<byte> local)
        {
            ulong k2 = 0;
            var lastIndex = (int)(count * readSize);
            if ((length & 15) == 15)
                k2 ^= (ulong)local[lastIndex + 14] << 48;
            if ((length & 15) >= 14)
                k2 ^= (ulong)local[lastIndex + 13] << 40;
            if ((length & 15) >= 13)
                k2 ^= (ulong)local[lastIndex + 12] << 32;
            if ((length & 15) >= 12)
                k2 ^= (ulong)local[lastIndex + 11] << 24;
            if ((length & 15) >= 11)
                k2 ^= (ulong)local[lastIndex + 10] << 16;
            if ((length & 15) >= 10)
                k2 ^= (ulong)local[lastIndex + 9] << 8;
            if ((length & 15) >= 9)
            {
                k2 ^= local[lastIndex + 8];
                k2 = MixKey2(k2);
            }

            return k2;
        }
    }

    private static ulong MixKey1(ulong k1)
    {
        k1 *= C1;
        k1 = k1.RotateLeft(31);
        k1 *= C2;
        return k1;
    }

    private static ulong MixKey2(ulong k2)
    {
        k2 *= C2;
        k2 = k2.RotateLeft(33);
        k2 *= C1;
        return k2;
    }

    private static ulong MixFinal(ulong value)
    {
        value ^= value >> 33;
        value *= 0xff51afd7ed558ccd;
        value ^= value >> 33;
        value *= 0xc4ceb9fe1a85ec53;
        value ^= value >> 33;

        return value;
    }

    private static ulong RotateLeft(this ulong value, int offset)
    {
        return (value << offset) | (value >> (64 - offset));
    }
}
