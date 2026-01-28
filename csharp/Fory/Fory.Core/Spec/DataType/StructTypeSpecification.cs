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
using System.Threading.Tasks;
using Fory.Core.Serializer;

namespace Fory.Core.Spec.DataType;

internal class StructTypeSpecification<TObject> : IStructTypeSpecification where TObject : struct
{
    public StructTypeSpecification(bool includeNamespace)
    {
        IsRegisteredByName = true;
        IsNamespaceIncluded = includeNamespace;
    }

    public StructTypeSpecification(uint typeId)
    {
        TypeId = typeId;
        IsRegisteredByName = false;
    }

    public Type AssociatedType => typeof(TObject);

    public uint TypeId { get; }

    public bool ReferenceTracking => false;

    public bool IsRegisteredByName { get; }

    public bool IsNamespaceIncluded { get; }

    public IForySerializer Serializer { get; }

    public Task Serialize(object value, SerializationContext context)
    {
        throw new NotImplementedException();
    }
}
