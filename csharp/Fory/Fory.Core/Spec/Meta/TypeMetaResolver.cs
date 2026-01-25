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
using System.Buffers;
using System.Linq;
using System.Reflection;
using Fory.Core.Encoding;
using Fory.Core.HashAlgorithm;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.DataType.Extensions;

namespace Fory.Core.Spec.Meta;

internal static class TypeMetaResolver
{
    /// <summary>
    ///     | global binary header: 8-bytes | meta header | fields meta |
    /// </summary>
    /// <param name="typeSpecification"></param>
    /// <param name="registry"></param>
    /// <returns></returns>
    public static byte[] Encode(ITypeSpecification typeSpecification, TypeSpecificationRegistry registry)
    {
        const int metaFieldRentLength = 500; // magic number
        var pool = ArrayPool<byte>.Shared;
        var metaFieldBuffer = pool.Rent(metaFieldRentLength);
        var metaHeaderLength = EncodeMetaHeader(typeSpecification, 0, ref metaFieldBuffer);
        var metaFieldLength = EncodeFieldsMeta(typeSpecification, registry, metaHeaderLength, ref metaFieldBuffer);
        var metaFieldSize = metaHeaderLength + metaFieldLength;
        var buffer = new byte[metaFieldSize + 8];
        var bufferPosition =
            EncodeGlobalBinaryHeader(metaFieldLength > 0, metaFieldSize, metaFieldBuffer, 0, ref buffer);
        metaFieldBuffer.CopyTo(buffer, bufferPosition);
        pool.Return(metaFieldBuffer, true);

        return buffer;
    }

    /// <summary>
    ///     | 50 bits hash | compress flag: 1-bit | write fields meta: 1-bit | meta size: 12-bits |
    /// </summary>
    /// <param name="hasFields"></param>
    /// <param name="metaFieldSize"></param>
    /// <param name="metaFieldBuffer"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeGlobalBinaryHeader(bool hasFields, int metaFieldSize, byte[] metaFieldBuffer,
        int bufferPosition, ref byte[] buffer)
    {
        const int metaFieldMask = 0xfff;

        ReadOnlySpan<byte> metaFieldSpan = metaFieldBuffer;
        var (h1, _) = MurmurHash3Algorithm.ComputeHash(ref metaFieldSpan, Constants.DefaultHashingSeed);

        var header = (long)h1;
        header <<= 14;
        header = Math.Abs(header);

        if (hasFields)
            header |= 1 << 12;

        var writtenByteLength = 0;
        var headerBuffer = BitConverter.GetBytes(header);
        headerBuffer.CopyTo(buffer, 0);
        writtenByteLength += headerBuffer.Length;
        bufferPosition += headerBuffer.Length;

        if (metaFieldSize >= metaFieldMask)
        {
            var remaining = (uint)(metaFieldSize - metaFieldMask);
            var remainingBytes = ForyEncoding.AsVarUInt32(remaining).ToArray();
            remainingBytes.CopyTo(buffer, bufferPosition);
            writtenByteLength += remainingBytes.Length;
        }

        return writtenByteLength;
    }

    /// <summary>
    ///     | unused: 2-bits | is registered by name flag: 1-bit | num fields: 4-bits |
    /// </summary>
    /// <param name="typeSpecification"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeMetaHeader(ITypeSpecification typeSpecification, int bufferPosition,
        ref byte[] buffer)
    {
        const byte smallNumFieldsThreshold = 0b11111;
        const byte registerByName = 0b100000;
        var writtenByteLength = 0;

        var isRegisteredByName = false;
        if (typeSpecification is IUserDefinedTypeSpecification userDefinedTypeSpecification)
            isRegisteredByName = userDefinedTypeSpecification.IsRegisteredByName;

        var numFields = typeSpecification.AssociatedType.GetProperties().Length;
        var metaHeader = (byte)Math.Min(numFields, smallNumFieldsThreshold);
        if (isRegisteredByName)
            metaHeader |= registerByName;

        buffer[bufferPosition] = metaHeader;
        bufferPosition += 1;
        writtenByteLength++;

        if (numFields > smallNumFieldsThreshold)
        {
            var remaining = (uint)numFields - smallNumFieldsThreshold;
            var remainingBytes = ForyEncoding.AsVarUInt32(remaining).ToArray();
            remainingBytes.CopyTo(buffer, bufferPosition);
            writtenByteLength += remainingBytes.Length;
            bufferPosition += remainingBytes.Length;
        }

        if (isRegisteredByName)
        {
            var typeNamespace = typeSpecification.AssociatedType.Namespace;
            var namespaceLength = EncodeNamespace(typeNamespace, bufferPosition, ref buffer);
            bufferPosition += namespaceLength;

            var typeName = typeSpecification.AssociatedType.Name;
            var typeNameLength = EncodeTypeName(typeName, bufferPosition, ref buffer);

            writtenByteLength += namespaceLength + typeNameLength;
        }
        else
        {
            var typeIdBytes = ForyEncoding.AsVarUInt32(typeSpecification.TypeId).ToArray();
            typeIdBytes.CopyTo(buffer, bufferPosition);

            writtenByteLength += typeIdBytes.Length;
        }

        return writtenByteLength;
    }

    /// <summary>
    /// </summary>
    /// <param name="typeNamespace"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeNamespace(string typeNamespace, int bufferPosition, ref byte[] buffer)
    {
        var encoding = NamespaceEncodingFactory.Instance.Value.GetEncoding(typeNamespace);
        var encodedLength =
            EncodeString(typeNamespace, encoding.Encoding, encoding.Flag, bufferPosition, ref buffer);

        return encodedLength;
    }

    /// <summary>
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeTypeName(string typeName, int bufferPosition, ref byte[] buffer)
    {
        var encoding = TypeNameEncodingFactory.Instance.Value.GetEncoding(typeName);
        var encodedLength = EncodeString(typeName, encoding.Encoding, encoding.Flag, bufferPosition, ref buffer);

        return encodedLength;
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="encoding"></param>
    /// <param name="encodingFlag"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeString(string name, System.Text.Encoding encoding, byte encodingFlag,
        int bufferPosition, ref byte[] buffer)
    {
        const byte bigNameThreshold = 0b111111;
        var writtenByteLength = 0;
        var pool = ArrayPool<byte>.Shared;
        var nameBuffer = pool.Rent(name.Length * (encodingFlag == 1 ? 2 : 1));
        var encodedNameLength = encoding.GetBytes(name, 0, name.Length, nameBuffer, 0);
        if (encodedNameLength >= bigNameThreshold)
        {
            buffer[bufferPosition] = (byte)((bigNameThreshold << 2) | encodingFlag);
            bufferPosition += 1;
            var remaining = (uint)encodedNameLength - bigNameThreshold;
            var encoded = ForyEncoding.AsVarUInt32(remaining).ToArray();
            encoded.CopyTo(buffer, bufferPosition);
            bufferPosition += encoded.Length;
            writtenByteLength += encoded.Length + 1;
        }
        else
        {
            buffer[bufferPosition] = (byte)((encodedNameLength << 2) | encodingFlag);
            writtenByteLength += 1;
        }

        nameBuffer.CopyTo(buffer, bufferPosition);
        writtenByteLength += encodedNameLength;

        return writtenByteLength;
    }

    /// <summary>
    ///     | field meta | next field meta | ... |
    /// </summary>
    /// <param name="typeSpecification"></param>
    /// <param name="typeSpecRegistry"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeFieldsMeta(ITypeSpecification typeSpecification,
        TypeSpecificationRegistry typeSpecRegistry, int bufferPosition, ref byte[] buffer)
    {
        var type = typeSpecification.AssociatedType;
        var totalLength = 0;
        foreach (var property in type.GetProperties())
            totalLength += EncodeFieldMeta(typeSpecRegistry, property, bufferPosition + totalLength, ref buffer);

        return totalLength;
    }

    /// <summary>
    ///     | field header: 8-bit | field type info | field name
    /// </summary>
    /// <param name="typeSpecRegistry"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="bufferPosition"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static int EncodeFieldMeta(TypeSpecificationRegistry typeSpecRegistry, PropertyInfo propertyInfo,
        int bufferPosition, ref byte[] buffer)
    {
        var propertyName = propertyInfo.Name;
        var encoding = FieldNameEncodingFactory.Instance.Value.GetEncoding(propertyName);
        var pool = ArrayPool<byte>.Shared;

        var fieldNameBuffer = pool.Rent(propertyName.Length * (encoding.Flag == 1 ? 2 : 1));
        var encodedNameLength =
            encoding.Encoding.GetBytes(propertyName, 0, propertyName.Length, fieldNameBuffer, 0);

        var fieldHeader = EncodeFieldHeader(propertyInfo, encodedNameLength - 1, encoding.Flag);
        var nullable = (fieldHeader & 2) == 2;

        const int fieldInfoRentLength = 100; // magic number
        var fieldInfoBuffer = pool.Rent(fieldInfoRentLength);
        var encodedFieldInfoLength = EncodeFieldInfo(typeSpecRegistry, propertyInfo.PropertyType, false, nullable,
            0, ref fieldInfoBuffer);

        var writtenByteLength = encodedNameLength + encodedFieldInfoLength + 1;
        var resizedLength = buffer.Length + writtenByteLength;
        if (buffer.Length < resizedLength)
            Array.Resize(ref buffer, resizedLength);

        buffer[bufferPosition] = fieldHeader;
        bufferPosition += 1;
        fieldInfoBuffer.CopyTo(buffer, bufferPosition);
        bufferPosition += encodedFieldInfoLength;
        fieldNameBuffer.CopyTo(buffer, bufferPosition);

        pool.Return(fieldNameBuffer, true);
        pool.Return(fieldInfoBuffer, true);

        return writtenByteLength;
    }

    /// <summary>
    ///     | field name encoding: 2-bits | field name size: 4-bits | nullability: 1-bit | reference tracking: 1-bit
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="encodedNameSize"></param>
    /// <param name="encodingFlag"></param>
    private static byte EncodeFieldHeader(PropertyInfo propertyInfo, int encodedNameSize, byte encodingFlag)
    {
        const int maxFieldNameSize = 15;
        var header = (byte)(encodingFlag << 6);
        header |= (byte)(Math.Min(maxFieldNameSize, encodedNameSize) << 2);
        var nullable = propertyInfo.PropertyType.IsNullable();
        if (nullable)
            header |= 2;

        return header;
    }

    /// <summary>
    ///     | field info | nested field info | ... |
    ///     Field info specification:
    ///     | type id | nullability: 1-bit | reference tracking: 1-bit |
    /// </summary>
    /// <param name="typeSpecRegistry">type specification registry</param>
    /// <param name="type">type info</param>
    /// <param name="isNestedType">true, if type info is the type of generic argument</param>
    /// <param name="nullable">true, if the type can be set to null</param>
    /// <param name="bufferPosition">buffer position</param>
    /// <param name="buffer">buffer</param>
    /// <returns>returns the number of written bytes in the buffer</returns>
    /// <exception cref="NotSupportedException">thrown when type info is not registered in the type specification registry</exception>
    private static int EncodeFieldInfo(TypeSpecificationRegistry typeSpecRegistry, Type type, bool isNestedType,
        bool nullable, int bufferPosition, ref byte[] buffer)
    {
        var typeSpec = typeSpecRegistry.GetTypeSpecification(type);
        var header = typeSpec.TypeId;
        if (isNestedType)
        {
            header <<= 2;
            if (nullable)
                header |= 2;
        }

        var encoded = ForyEncoding.AsVarUInt32(header).ToArray();
        if (bufferPosition + encoded.Length >= buffer.Length)
            Array.Resize(ref buffer, buffer.Length * 2);
        encoded.CopyTo(buffer, bufferPosition);
        bufferPosition += encoded.Length;

        if (!Enum.IsDefined(typeof(TypeSpecificationRegistry.KnownTypes), typeSpec.TypeId))
            return 1;

        var knownType = (TypeSpecificationRegistry.KnownTypes)typeSpec.TypeId;
        switch (knownType)
        {
            case TypeSpecificationRegistry.KnownTypes.List:
            case TypeSpecificationRegistry.KnownTypes.Set:
                var typeParam = type.GetGenericArguments()[0];
                return 1 + EncodeFieldInfo(typeSpecRegistry, typeParam, true, typeParam.IsNullable(),
                    bufferPosition, ref buffer);
            case TypeSpecificationRegistry.KnownTypes.Map:
                var arguments = type.GetGenericArguments();
                var keyBufferLength = EncodeFieldInfo(typeSpecRegistry, arguments[0], true,
                    arguments[0].IsNullable(), bufferPosition, ref buffer);
                var valueBufferLength = EncodeFieldInfo(typeSpecRegistry, arguments[1], true,
                    arguments[1].IsNullable(), bufferPosition + 1, ref buffer);
                return 1 + keyBufferLength + valueBufferLength;
            default:
                return 1;
        }
    }

    private static bool IsNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }
}
