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

using System.Runtime.Serialization;
using Fory.Core.Spec;

namespace Fory.Core.Extensions;

public static class HeaderInfoExtensions
{
    public static bool IsHeaderValidOrThrow(this HeaderInfo headerInfo, DeserializationContext context)
    {
        if (headerInfo.IsPeerXlang != context.IsXlang)
            throw new SerializationException(
                "Mismatch found in header bitmap between xlang bit and current Fory configuration.");

        if (!headerInfo.IsPeerLittleEdian)
            throw new SerializationException("Big endian is currently not supported");

        return true;
    }
}
