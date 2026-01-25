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
using System.Buffers;
using System.Text;

namespace Fory.Core.Encoding;

internal class AllToLowerSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
{
    private static readonly Encoder DefaultEncoder = new FiveBitMetaStringEncoder();

    public bool Evaluate(StringStatistics stats, string value)
    {
        return stats.CanLowerUpperDigitSpecialEncode && (value.Length + stats.UpperCount) * 5 < value.Length * 6;
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        var pool = ArrayPool<byte>.Shared;
        var rent = pool.Rent(count * 2);
        var byteCount = GetBytes(chars, index, count, rent, 0);
        pool.Return(rent, true);

        return byteCount;
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        var pool = ArrayPool<char>.Shared;
        // Rent at least two times the input array size, making sure we have enough space in case all characters are capitalized.
        var rent = pool.Rent(charCount * 2);
        var pos = 0;
        foreach (var c in chars)
        {
            if (char.IsUpper(c))
            {
                rent[pos++] = '|';
                rent[pos++] = char.ToLower(c);
                continue;
            }

            rent[pos++] = c;
        }

        var written = DefaultEncoder.GetBytes(rent, charIndex, charCount, bytes, byteIndex, false);
        pool.Return(rent, true);

        return written;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxByteCount(int charCount)
    {
        throw new NotImplementedException();
    }

    public override int GetMaxCharCount(int byteCount)
    {
        throw new NotImplementedException();
    }
}
