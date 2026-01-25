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
using System.Collections.Generic;
using Fory.Core.Spec.DataType;

namespace Fory.Core.Spec.Meta;

internal class TypeMetaRegistry
{
    private readonly Queue<byte[]> _typeDefinitions;
    private readonly Dictionary<Type, uint> _typeIndexMap;
    private readonly TypeSpecificationRegistry _typeSpecificationRegistry;

    internal TypeMetaRegistry(TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _typeDefinitions = new Queue<byte[]>();
        _typeIndexMap = new Dictionary<Type, uint>();

        _typeSpecificationRegistry = typeSpecificationRegistry;
    }

    public uint TryRegister(ITypeSpecification typeSpecification)
    {
        var type = typeSpecification.AssociatedType;
        if (_typeIndexMap.TryGetValue(type, out var index))
            return index;

        _typeDefinitions.Enqueue(TypeMetaResolver.Encode(typeSpecification, _typeSpecificationRegistry));

        index = (uint)_typeIndexMap.Count;
        _typeIndexMap.Add(type, index);
        return index;
    }
}
