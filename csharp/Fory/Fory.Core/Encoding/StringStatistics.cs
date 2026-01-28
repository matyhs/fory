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

using System.Linq;

namespace Fory.Core.Encoding;

internal struct StringStatistics
{
    public uint DigitCount { get; private set; }
    public uint UpperCount { get; private set; }
    public bool CanLowerUpperDigitSpecialEncode { get; private set; }
    public bool CanLowerSpecialEncode { get; private set; }

    internal static StringStatistics GetStats(string value, params char[] specialCharacters)
    {
        var stats = new StringStatistics
        {
            CanLowerSpecialEncode = true,
            CanLowerUpperDigitSpecialEncode = true
        };

        foreach (var c in value)
        {
            if (stats.CanLowerUpperDigitSpecialEncode &&
                !(char.IsLower(c) || char.IsUpper(c) || char.IsDigit(c) || specialCharacters.Contains(c)))
                stats.CanLowerUpperDigitSpecialEncode = false;

            if (stats.CanLowerSpecialEncode &&
                !(char.IsLower(c) || c == '.' || c == '_' || c == '$' || c == '|'))
                stats.CanLowerSpecialEncode = false;

            if (char.IsDigit(c))
                stats.DigitCount++;

            if (char.IsUpper(c))
                stats.UpperCount++;
        }

        return stats;
    }
}
