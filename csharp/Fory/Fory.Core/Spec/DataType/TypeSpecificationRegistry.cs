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
using System.Linq.Expressions;
using Fory.Core.SourceGenerator;

namespace Fory.Core.Spec.DataType;

internal class TypeSpecificationRegistry
{
    public enum KnownTypes : ushort
    {
        /// <summary>
        ///     a boolean value (true or false).
        /// </summary>
        [PrimitiveTypeSpecification<bool>] Boolean = 1,

        /// <summary>
        ///     a 8-bit signed integer.
        /// </summary>
        [PrimitiveTypeSpecification<sbyte>] Int8 = 2,

        /// <summary>
        ///     a 16-bit signed integer.
        /// </summary>
        [PrimitiveTypeSpecification<short>] Int16 = 3,

        /// <summary>
        ///     a 32-bit signed integer.
        /// </summary>
        [PrimitiveTypeSpecification<int>] Int32 = 4,

        /// <summary>
        ///     a 32-bit signed integer which use fory var_int32 encoding.
        /// </summary>
        VarInt32 = 5,

        /// <summary>
        ///     a 64-bit signed integer.
        /// </summary>
        [PrimitiveTypeSpecification<long>] Int64 = 6,

        /// <summary>
        ///     a 64-bit signed integer which use fory PVL encoding.
        /// </summary>
        VarInt64 = 7,

        /// <summary>
        ///     a 64-bit signed integer which use fory SLI encoding.
        /// </summary>
        SliInt64 = 8,

        /// <summary>
        ///     a 16-bit floating point number.
        /// </summary>
#if NET
        [PrimitiveTypeSpecification<Half>]
#endif
        // TODO: Would it make sense to support this for .NET standard?
        Float16 = 9,

        /// <summary>
        ///     a 32-bit floating point number.
        /// </summary>
        [PrimitiveTypeSpecification<float>] Float32 = 10,

        /// <summary>
        ///     a 64-bit floating point number including NaN and Infinity.
        /// </summary>
        [PrimitiveTypeSpecification<double>] Float64 = 11,

        /// <summary>
        ///     a text string encoded using Latin1/UTF16/UTF-8 encoding.
        /// </summary>
        [PrimitiveTypeSpecification<string>(FullyQualifiedSerializerTypeName = "Fory.Core.Serializer.StringSerializer")]
        String = 12,

        /// <summary>
        ///     a data type consisting of a set of named values. Rust enum with non-predefined field values are not supported as an
        ///     enum.
        /// </summary>
        Enum = 13,

        /// <summary>
        ///     an enum whose value will be serialized as the registered name.
        /// </summary>
        NamedEnum = 14,

        /// <summary>
        ///     a morphic(final) type serialized by Fory Struct serializer. i.e. it doesn't have subclasses. Suppose we're
        ///     deserializing List&lt;SomeClass&gt;, we can save dynamic serializer dispatch since SomeClass is morphic(final).
        /// </summary>
        Struct = 15,

        /// <summary>
        ///     a morphic(final) type serialized by Fory compatible Struct serializer.
        /// </summary>
        CompatibleStruct = 16,

        /// <summary>
        ///     a struct whose type mapping will be encoded as a name.
        /// </summary>
        NamedStruct = 17,

        /// <summary>
        ///     a compatible_struct whose type mapping will be encoded as a name.
        /// </summary>
        NamedCompatibleStruct = 18,

        /// <summary>
        ///     a type which will be serialized by a customized serializer.
        /// </summary>
        Ext = 19,

        /// <summary>
        ///     an ext type whose type mapping will be encoded as a name.
        /// </summary>
        NamedExt = 20,

        /// <summary>
        ///     a sequence of objects.
        /// </summary>
        List = 21,

        /// <summary>
        ///     an unordered set of unique elements.
        /// </summary>
        Set = 22,

        /// <summary>
        ///     a map of key-value pairs. Mutable types such as list/map/set/array/tensor/arrow are not allowed as key of map.
        /// </summary>
        Map = 23,

        /// <summary>
        ///     an absolute length of time, independent of any calendar/timezone, as a count of nanoseconds.
        /// </summary>
        [PrimitiveTypeSpecification<TimeSpan>(FullyQualifiedSerializerTypeName =
            "Fory.Core.Serializer.DurationSerializer")]
        Duration = 24,

        /// <summary>
        ///     a point in time, independent of any calendar/timezone, as a count of nanoseconds. The count is relative to an epoch
        ///     at UTC midnight on January 1, 1970.
        /// </summary>
        [PrimitiveTypeSpecification<DateTimeOffset>(FullyQualifiedSerializerTypeName =
            "Fory.Core.Serializer.TimestampSerializer")]
        Timestamp = 25,

        /// <summary>
        ///     a naive date without timezone. The count is days relative to an epoch at UTC midnight on Jan 1, 1970.
        /// </summary>
        [PrimitiveTypeSpecification<DateTime>(FullyQualifiedSerializerTypeName =
            "Fory.Core.Serializer.LocalDateSerializer")]
        LocalDate = 26,

        /// <summary>
        ///     exact decimal value represented as an integer value in two's complement.
        /// </summary>
        Decimal = 27,

        /// <summary>
        ///     an variable-length array of bytes.
        /// </summary>
        Binary = 28,

        /// <summary>
        ///     only allow 1d numeric components. Other arrays will be taken as List. The implementation should support the
        ///     interoperability between array and list.
        /// </summary>
        Array = 29,

        /// <summary>
        ///     one dimensional boolean array.
        /// </summary>
        BoolArray = 30,

        /// <summary>
        ///     one dimensional int8 array.
        /// </summary>
        Int8Array = 31,

        /// <summary>
        ///     one dimensional int16 array.
        /// </summary>
        Int16Array = 32,

        /// <summary>
        ///     one dimensional int32 array.
        /// </summary>
        Int32Array = 33,

        /// <summary>
        ///     one dimensional int64 array.
        /// </summary>
        Int64Array = 34,

        /// <summary>
        ///     one dimensional half_float_16 array.
        /// </summary>
        Float16Array = 35,

        /// <summary>
        ///     one dimensional float32 array.
        /// </summary>
        Float32Array = 36,

        /// <summary>
        ///     one dimensional float64 array.
        /// </summary>
        Float64Array = 37,

        /// <summary>
        ///     multidimensional array which every sub-array have same size and type.
        /// </summary>
        Tensor = 38,

        /// <summary>
        ///     an arrow record batch object.
        /// </summary>
        ArrowRecordBatch = 39,

        /// <summary>
        ///     an arrow table object.
        /// </summary>
        ArrowTable = 40,

        /// <summary>
        ///     8-bit unsigned integer
        /// </summary>
        [PrimitiveTypeSpecification<byte>] UInt8 = 64,

        /// <summary>
        ///     16-bit unsigned integer
        /// </summary>
        [PrimitiveTypeSpecification<ushort>] UInt16 = 65,

        /// <summary>
        ///     32-bit unsigned integer
        /// </summary>
        [PrimitiveTypeSpecification<uint>] UInt32 = 66,

        /// <summary>
        ///     64-bit unsigned integer
        /// </summary>
        [PrimitiveTypeSpecification<ulong>] UInt64 = 68
    }

    private static readonly TypeSpecificationFactory Factory = new();

    private readonly Dictionary<Type, ITypeSpecification> _registryByType;
    private readonly Dictionary<uint, ITypeSpecification> _registryByTypeId;

    internal TypeSpecificationRegistry()
    {
        _registryByType = new Dictionary<Type, ITypeSpecification>();
        _registryByTypeId = new Dictionary<uint, ITypeSpecification>();

        RegisterInternal<BooleanTypeSpecification>();
        RegisterInternal<Int8TypeSpecification>();
        RegisterInternal<Int16TypeSpecification>();
        RegisterInternal<Int32TypeSpecification>();
        RegisterInternal<Int64TypeSpecification>();
#if NET
        RegisterInternal<Float16TypeSpecification>();
#endif
        RegisterInternal<Float32TypeSpecification>();
        RegisterInternal<Float64TypeSpecification>();
        RegisterInternal<UInt8TypeSpecification>();
        RegisterInternal<UInt16TypeSpecification>();
        RegisterInternal<UInt32TypeSpecification>();
        RegisterInternal<UInt64TypeSpecification>();
        RegisterInternal<StringTypeSpecification>();
        RegisterInternal<DurationTypeSpecification>();
        RegisterInternal<TimestampTypeSpecification>();
        RegisterInternal<LocalDateTypeSpecification>();
    }

    public ITypeSpecification this[Type type] => _registryByType[type];

    public bool TryGetTypeSpecification(Type type, out ITypeSpecification typeSpec)
    {
        return _registryByType.TryGetValue(type, out typeSpec);
    }

    public bool TryGetTypeSpecification(uint typeId, out ITypeSpecification typeSpec)
    {
        return _registryByTypeId.TryGetValue(typeId, out typeSpec);
    }

    public void Register<TObject>(uint typeId)
    {
        var typeSpec = Factory.Create<TObject>(typeId);
        RegisterInternal(typeSpec);
    }

    public void Register<TObject>(bool includeNamespace)
    {
        var typeSpec = Factory.Create<TObject>(includeNamespace);
        RegisterInternal(typeSpec);
    }

    private void RegisterInternal<TTypeSpec>() where TTypeSpec : IKnownTypeSpecification, new()
    {
        var typeSpec = new TTypeSpec();
        RegisterInternal(typeSpec);
    }

    private void RegisterInternal(ITypeSpecification typeMetaSpec)
    {
        _registryByType.Add(typeMetaSpec.AssociatedType, typeMetaSpec);
        _registryByTypeId.Add(typeMetaSpec.TypeId, typeMetaSpec);
    }

    private class TypeSpecificationFactory
    {
        /// <summary>
        ///     Create a type specification with a given type id
        /// </summary>
        /// <param name="typeId">type id</param>
        /// <typeparam name="TObject">user-defined object type</typeparam>
        /// <returns>type specification for the user-defined object type</returns>
        public ITypeSpecification Create<TObject>(uint typeId)
        {
            if (typeof(TObject).IsEnum)
            {
                var enumType = typeof(EnumTypeSpecification<>).MakeGenericType(typeof(TObject));
                return CreateInternal(enumType, typeId);
            }

            var structType = typeof(StructTypeSpecification<>).MakeGenericType(typeof(TObject));
            return CreateInternal(structType, typeId);
        }

        /// <summary>
        ///     Create a type specification based on type name and/or type namespace
        /// </summary>
        /// <param name="includeNamespace">includes namespace</param>
        /// <typeparam name="TObject">user-defined object type</typeparam>
        /// <returns>type specification for the user-defined object type</returns>
        public ITypeSpecification Create<TObject>(bool includeNamespace)
        {
            if (typeof(TObject).IsEnum)
            {
                var enumType = typeof(EnumTypeSpecification<>).MakeGenericType(typeof(TObject));
                return CreateInternal(enumType, includeNamespace);
            }

            var structType = typeof(StructTypeSpecification<>).MakeGenericType(typeof(TObject));
            return CreateInternal(structType, includeNamespace);
        }

        private static ITypeSpecification CreateInternal(Type typeSpecType, uint typeId)
        {
            var constructor = typeSpecType.GetConstructor([typeof(uint)]);
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            var typeIdParam = Expression.Constant(typeId);
            var newTypeSpec = Expression.New(constructor, typeIdParam);
            return Expression.Lambda<Func<ITypeSpecification>>(newTypeSpec).Compile()();
        }

        private static ITypeSpecification CreateInternal(Type typeSpecType, bool includeNamespace)
        {
            var constructor = typeSpecType.GetConstructor([typeof(bool)]);
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            var typeIdParam = Expression.Constant(includeNamespace);
            var newTypeSpec = Expression.New(constructor, typeIdParam);
            return Expression.Lambda<Func<ITypeSpecification>>(newTypeSpec).Compile()();
        }
    }
}
