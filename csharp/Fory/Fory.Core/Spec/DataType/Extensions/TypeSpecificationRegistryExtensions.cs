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

namespace Fory.Core.Spec.DataType.Extensions;

internal static class TypeSpecificationRegistryExtensions
{
    /// <summary>
    ///     Get the registered type specification. This extension method allows a fallback type specification retrieval in the
    ///     case of generic types registration.
    /// </summary>
    /// <param name="registry">type specification registry</param>
    /// <param name="type">type to locate</param>
    /// <returns>returns the type specification of the given type</returns>
    /// <exception cref="NotSupportedException">thrown when type is not registered</exception>
    public static ITypeSpecification GetTypeSpecification(this TypeSpecificationRegistry registry, Type type)
    {
        var exception = new NotSupportedException($"Type is not registered: {type.FullName}");
        return registry.TryGetTypeSpecification(type, out var typeSpec)
            ? typeSpec
            : type.ContainsGenericParameters
                ? registry.TryGetTypeSpecification(type.GetGenericTypeDefinition(), out typeSpec)
                    ? typeSpec
                    : throw exception
                : throw exception;
    }
}
