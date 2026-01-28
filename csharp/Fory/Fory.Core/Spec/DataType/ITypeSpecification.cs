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

/// <summary>
///     Defines the type specification for a data type
/// </summary>
internal interface ITypeSpecification
{
    Type AssociatedType { get; }

    uint TypeId { get; }

    bool ReferenceTracking { get; }

    IForySerializer Serializer { get; }
}

internal interface ITypeSpecification<TType> : ITypeSpecification
{
    new IForySerializer<TType> Serializer { get; }
}

/// <summary>
///     Defines the type specification for system-defined data type
/// </summary>
internal interface IKnownTypeSpecification : ITypeSpecification
{
    TypeSpecificationRegistry.KnownTypes KnownTypeId { get; }
}

internal interface IKnownTypeSpecification<TType> : IKnownTypeSpecification, ITypeSpecification<TType>
{
}

/// <summary>
///     Defines user-defined type specification
/// </summary>
internal interface IUserDefinedTypeSpecification : ITypeSpecification
{
    bool IsRegisteredByName { get; }

    bool IsNamespaceIncluded { get; }
}

/// <summary>
///     Defines the type specification for enum data type
/// </summary>
internal interface IEnumTypeSpecification : IUserDefinedTypeSpecification
{
}

/// <summary>
///     Defines the type specification for POCO
/// </summary>
internal interface IStructTypeSpecification : IUserDefinedTypeSpecification
{
}

/// <summary>
///     Defines the type specification for POCO with custom serializer
/// </summary>
internal interface IExtTypeSpecification : IUserDefinedTypeSpecification
{
    Type SerializerType { get; }
}
