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

namespace Fory.Core.Encoding;

public class FieldNameEncodingFactory : IEncodingFactory
{
    private static readonly System.Text.Encoding Utf8Encoding = System.Text.Encoding.UTF8;

    private static readonly AllToLowerSpecialMetaStringEncoding AllToLowerSpecialEncoding = new();

    private static readonly LowerUpperDigitSpecialMetaStringEncoding LowerUpperDigitSpecialEncoding = new();

    public static readonly Lazy<IEncodingFactory> Instance = new(() => new FieldNameEncodingFactory());

    public (System.Text.Encoding Encoding, byte Flag) GetEncoding(string value)
    {
        var stats = StringStatistics.GetStats(value);
        if (LowerUpperDigitSpecialEncoding.Evaluate(stats, value))
            return (LowerUpperDigitSpecialEncoding, AsBitFieldNameEncoding(2));

        if (AllToLowerSpecialEncoding.Evaluate(stats, value))
            return (AllToLowerSpecialEncoding, AsBitFieldNameEncoding(1));

        return (Utf8Encoding, 0);
    }

    private static byte AsBitFieldNameEncoding(byte flag)
    {
        return (byte)(flag << 6);
    }
}
