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
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType;

internal class ExtTypeSpecification<TSerializer> : IExtTypeSpecification
    where TSerializer : IForySerializer, new()
{
    private readonly Lazy<TSerializer> _serializer;

    public ExtTypeSpecification(bool includeNamespace) : this()
    {
        IsRegisteredByName = true;
        IsNamespaceIncluded = includeNamespace;
    }

    public ExtTypeSpecification(uint typeId) : this()
    {
        TypeId = typeId;
        IsRegisteredByName = false;
    }

    private ExtTypeSpecification()
    {
        _serializer = new Lazy<TSerializer>(() => new TSerializer());
    }

    public Type SerializerType => typeof(TSerializer);
    public Type AssociatedType => _serializer.Value.AssociatedType;
    public uint TypeId { get; }
    public bool ReferenceTracking => true;
    public bool IsRegisteredByName { get; }
    public bool IsNamespaceIncluded { get; }
    public IForySerializer Serializer => _serializer.Value;
}
