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

namespace Fory.Core.Spec;

internal class ForyHeaderSpec
{
    [Flags]
    public enum ForyConfigFlags : byte
    {
        None = 0,
        IsNull = 1,
        IsXlang = 1 << 1,
        IsOob = 1 << 2
    }

    public const byte LanguageCode = 0x8;
}

public readonly record struct HeaderInfo
{
    private readonly ForyHeaderSpec.ForyConfigFlags _bitmap;

    internal HeaderInfo(ForyHeaderSpec.ForyConfigFlags bitmap)
    {
        _bitmap = bitmap;
    }

    internal HeaderInfo(ForyHeaderSpec.ForyConfigFlags bitmap, byte languageCode) : this(bitmap)
    {
        SourceLanguageCode = languageCode;
    }

    public byte? SourceLanguageCode { get; }

    public bool IsPeerXlang => _bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsXlang);

    public bool IsNull => _bitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsNull);
}
