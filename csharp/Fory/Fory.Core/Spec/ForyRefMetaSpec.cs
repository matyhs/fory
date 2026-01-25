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

namespace Fory.Core.Spec;

internal static class ForyRefMetaSpec
{
    public enum ReferenceFlag : sbyte
    {
        // This flag indicates the object is a null value. We don't use another byte to indicate REF, so that we can save one byte.
        Null = -3,

        // This flag indicates the object is already serialized previously, and fory will write a ref id with unsigned varint format instead of serialize it again
        Ref = -2,

        // This flag indicates the object is a non-null value and fory doesn't track ref for this type of object.
        NotNull = -1,

        // This flag indicates the object is referencable and the first time to serialize.
        RefValue = 0
    }
}

public readonly struct ReferenceInfo
{
    private readonly ForyRefMetaSpec.ReferenceFlag _bitmap;

    public bool IsNull => _bitmap == ForyRefMetaSpec.ReferenceFlag.Null;

    public bool IsNewReference => _bitmap == ForyRefMetaSpec.ReferenceFlag.RefValue;

    public bool HasReference => _bitmap == ForyRefMetaSpec.ReferenceFlag.Ref;

    internal ReferenceInfo(ForyRefMetaSpec.ReferenceFlag bitmap)
    {
        _bitmap = bitmap;
    }
}
