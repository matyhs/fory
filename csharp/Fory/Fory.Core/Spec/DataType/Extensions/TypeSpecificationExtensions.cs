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

namespace Fory.Core.Spec.DataType.Extensions;

internal static class TypeSpecificationExtensions
{
    internal static ushort GetTypeId(this IKnownTypeSpecification typeSpec)
    {
        return (ushort)typeSpec.TypeId;
    }

    internal static uint GetTypeId(this IEnumTypeSpecification typeSpec)
    {
        return typeSpec.IsRegisteredByName
            ? (uint)TypeSpecificationRegistry.KnownTypes.NamedEnum
            : (typeSpec.TypeId << 8) +
              (uint)TypeSpecificationRegistry.KnownTypes
                  .Enum; // Shift left to reserve the first 8 bits to represent the fory type id
    }

    internal static uint GetTypeId(this IStructTypeSpecification typeSpec, bool compatible = false)
    {
        if (compatible)
            return typeSpec.IsRegisteredByName
                ? (uint)TypeSpecificationRegistry.KnownTypes.NamedCompatibleStruct
                : (typeSpec.TypeId << 8) + (uint)TypeSpecificationRegistry.KnownTypes.CompatibleStruct;

        if (typeSpec.IsRegisteredByName) return (uint)TypeSpecificationRegistry.KnownTypes.NamedStruct;

        return (typeSpec.TypeId << 8) + (uint)TypeSpecificationRegistry.KnownTypes.Struct;
    }

    internal static uint GetTypeId(this IExtTypeSpecification typeSpec)
    {
        var foryKnownType = typeSpec.IsRegisteredByName
            ? TypeSpecificationRegistry.KnownTypes.NamedExt
            : TypeSpecificationRegistry.KnownTypes.Ext;

        return (typeSpec.TypeId << 8) + (uint)foryKnownType;
    }

    internal static string GetNamespace(this IUserDefinedTypeSpecification typeSpec)
    {
        return typeSpec.IsNamespaceIncluded ? typeSpec.AssociatedType.Namespace ?? string.Empty : string.Empty;
    }
}
