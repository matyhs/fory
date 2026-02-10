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
using System.Text;

namespace Fory.Core.Encoding;

internal class FirstToLowerSpecialMetaStringEncoding : System.Text.Encoding, IEncodingRule
{
    private static readonly Encoder DefaultEncoder = new FiveBitMetaStringEncoder();

    public bool Evaluate(StringStatistics stats, string value)
    {
        return stats.UpperCount == 1 && char.IsUpper(value[0]);
    }

    public override Encoder GetEncoder()
    {
        return new FiveBitMetaStringEncoder();
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        return DefaultEncoder.GetByteCount(chars, index, count, false);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        var pool = ArrayPool<char>.Shared;
        var rent = pool.Rent(charCount);
        for (var i = 0; i < chars.Length; i++) rent[i] = i == 0 ? char.ToLower(chars[i]) : chars[i];

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
