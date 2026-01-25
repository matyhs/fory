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
using Fory.Core.Encoding;
using Fory.Core.HashAlgorithm;
using Fory.Core.Spec.DataType;

namespace Fory.Core.Spec.Meta;

internal class TypeMetaStringRegistry
{
    private readonly Dictionary<MetaStringBytes, uint> _metaStringBytesIndexMap = new();
    private readonly Dictionary<int, MetaStringBytes> _stringMetaBytesMap = new();

    public byte[] TryRegister(ITypeSpecification typeSpecification)
    {
        var metaStringBytes = GetOrCreate(typeSpecification.AssociatedType.Namespace,
            NamespaceEncodingFactory.Instance.Value);
        var isRegistered = TryRegister(metaStringBytes, out var index);
        var namespaceBuffer = TypeMetaStringResolver.Encode(index, isRegistered ? null : metaStringBytes);

        metaStringBytes = GetOrCreate(typeSpecification.AssociatedType.Name,
            TypeNameEncodingFactory.Instance.Value);
        isRegistered = TryRegister(metaStringBytes, out index);
        var typeNameBuffer = TypeMetaStringResolver.Encode(index, isRegistered ? null : metaStringBytes);

        var buffer = new byte[namespaceBuffer.Length + typeNameBuffer.Length];
        namespaceBuffer.CopyTo(buffer, 0);
        typeNameBuffer.CopyTo(buffer, namespaceBuffer.Length);

        return buffer;
    }

    private MetaStringBytes GetOrCreate(string metaString, IEncodingFactory encodingFactory)
    {
        const ulong hashFlag = 0xffffffffffffff00;
        const byte encodingFlag = 0xff;
        var (encoding, flag) = encodingFactory.GetEncoding(metaString);
        var bytes = encoding.GetBytes(metaString);
        var byteHash = bytes.GetHashCode();
        if (!_stringMetaBytesMap.TryGetValue(byteHash, out var cachedEntry))
        {
            ReadOnlySpan<byte> byteSpan = bytes;
            var (h1, _) = MurmurHash3Algorithm.ComputeHash(ref byteSpan, Constants.DefaultHashingSeed);
            var convertedHash = (long)h1;
            convertedHash = Math.Abs(convertedHash);
            if (convertedHash == 0)
                convertedHash += 256;
            convertedHash = (long)((ulong)convertedHash & hashFlag);
            var header = (long)flag & encodingFlag;
            convertedHash |= header;

            cachedEntry = new MetaStringBytes(bytes, flag, convertedHash);
            _stringMetaBytesMap.Add(byteHash, cachedEntry);
        }

        return cachedEntry;
    }

    private bool TryRegister(MetaStringBytes metaStringBytes, out uint metaStringBytesIndex)
    {
        if (!_metaStringBytesIndexMap.TryGetValue(metaStringBytes, out metaStringBytesIndex))
        {
            _metaStringBytesIndexMap.Add(metaStringBytes, (uint)_stringMetaBytesMap.Count);
            return true;
        }

        return false;
    }
}
