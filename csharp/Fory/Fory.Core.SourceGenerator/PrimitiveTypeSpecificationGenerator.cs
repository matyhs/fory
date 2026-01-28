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

using System.Text;
using Fory.Core.SourceGenerator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Fory.Core.SourceGenerator;

[Generator]
public class PrimitiveTypeSpecificationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateAttribute);

        var fullyQualifiedName = "Fory.Core.SourceGenerator.PrimitiveTypeSpecificationAttribute`1";
        var typeSpecificationToGenerate =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                    fullyQualifiedName,
                    IsTargetForGeneration,
                    GetTargetForGeneration
                )
                .Where(info => info is not null);

        context.RegisterSourceOutput(typeSpecificationToGenerate, GenerateSerializer);
        context.RegisterSourceOutput(typeSpecificationToGenerate, GenerateTypeSpecification);
    }

    private static void GenerateAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        const string attributeFileName = "PrimitiveTypeSpecificationAttribute.g.cs";
        const string attributeCode = """

                                     namespace Fory.Core.SourceGenerator
                                     {
                                         [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
                                         [System.AttributeUsage(System.AttributeTargets.Field)]
                                         internal sealed class PrimitiveTypeSpecificationAttribute<TType> : global::System.Attribute
                                         {
                                             public string? TypeName { get; set; }
                                             public string? FullyQualifiedSerializerTypeName { get; set; }
                                         }
                                     }

                                     """;
        context.AddEmbeddedAttributeDefinition();
        context.AddSource(attributeFileName, attributeCode);
    }

    private static bool IsTargetForGeneration(SyntaxNode syntax, CancellationToken cancellationToken)
    {
        return true;
    }

    private static PrimitiveTypeSpecificationInfo? GetTargetForGeneration(GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        foreach (var attribute in context.Attributes)
        {
            if (attribute.AttributeClass is null ||
                !attribute.AttributeClass.Name.StartsWith("PrimitiveTypeSpecificationAttribute"))
                continue;

            var enumName = context.TargetSymbol.Name;
            var typeName = attribute.NamedArguments.FirstOrDefault(x => x.Key == "TypeName").Value.Value?.ToString();
            var fullyQualifiedSerializerTypeName = attribute.NamedArguments
                .FirstOrDefault(x => x.Key == "FullyQualifiedSerializerTypeName").Value.Value?.ToString();
            var typeRepresentation = attribute.AttributeClass.TypeArguments[0];
            var typeAlias = typeRepresentation.ToDisplayString();
            var isBuiltInUnmanagedType = typeRepresentation.IsBuiltInUnmanagedType();

            return new PrimitiveTypeSpecificationInfo(typeName, typeAlias, enumName, isBuiltInUnmanagedType,
                fullyQualifiedSerializerTypeName);
        }

        return null;
    }

    private static void GenerateSerializer(SourceProductionContext productionContext,
        PrimitiveTypeSpecificationInfo? generationInfo)
    {
        if (!string.IsNullOrEmpty(generationInfo!.Value.FullyQualifiedSerializerTypeName))
            return;

        var typeAlias = generationInfo!.Value.TypeAlias;
        var typeSizeDefinition = generationInfo.Value.IsBuiltInUnmanagedType
            ? $"const int typeSize = sizeof({typeAlias})"
            : $"var typeSize = System.Runtime.InteropServices.Marshal.SizeOf<{typeAlias}>()";
        var sourceCode = $$"""

                           namespace Fory.Core.SourceGenerator
                           {
                               internal class {{generationInfo.Value.SerializerName}} : global::Fory.Core.Serializer.ForySerializerBase<{{typeAlias}}> {
                                   public override async System.Threading.Tasks.Task SerializeDataAsync({{typeAlias}} value, global::Fory.Core.SerializationContext context,
                                       System.Threading.CancellationToken cancellationToken = default)
                                   {
                                        {{typeSizeDefinition}};
                                        var span = context.Writer.GetSpan();
                                        System.Runtime.InteropServices.MemoryMarshal.Write(span, ref value);
                                        context.Writer.Advance(typeSize);

                                        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                                   }

                                   public override async System.Threading.Tasks.ValueTask<{{typeAlias}}> DeserializeDataAsync(global::Fory.Core.DeserializationContext context,
                                       System.Threading.CancellationToken cancellationToken = default)
                                   {
                                        {{typeSizeDefinition}};
                                        var readResult = await context.Reader.ReadAsync(cancellationToken);
                                        var sequence = readResult.Buffer.Slice(0, typeSize);
                                        var value = System.Runtime.InteropServices.MemoryMarshal.Read<{{typeAlias}}>(sequence.First.Span);
                                        context.Reader.AdvanceTo(sequence.End);

                                        return value;
                                   }
                               }
                           }

                           """;
        productionContext.AddSource($"{generationInfo.Value.SerializerName}.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static void GenerateTypeSpecification(SourceProductionContext productionContext,
        PrimitiveTypeSpecificationInfo? generationInfo)
    {
        var typeAlias = generationInfo!.Value.TypeAlias;
        var sourceCode = $$"""

                           namespace Fory.Core.SourceGenerator
                           {
                               internal class {{generationInfo.Value.TypeSpecificationName}} : global::Fory.Core.Spec.DataType.IKnownTypeSpecification<{{typeAlias}}>
                               {
                                   private readonly System.Lazy<global::Fory.Core.Serializer.IForySerializer<{{typeAlias}}>> _serializer = new(() => new {{generationInfo.Value.SerializerName}}());

                                   public System.Type AssociatedType => typeof({{typeAlias}});
                                   public uint TypeId => (uint)KnownTypeId;
                                   public bool ReferenceTracking => false;
                                   public global::Fory.Core.Spec.DataType.TypeSpecificationRegistry.KnownTypes KnownTypeId => global::Fory.Core.Spec.DataType.TypeSpecificationRegistry.KnownTypes.{{generationInfo.Value.EnumName}};
                                   global::Fory.Core.Serializer.IForySerializer global::Fory.Core.Spec.DataType.ITypeSpecification.Serializer => Serializer;
                                   public global::Fory.Core.Serializer.IForySerializer<{{typeAlias}}> Serializer => _serializer.Value;
                               }
                           }

                           """;
        productionContext.AddSource($"{generationInfo.Value.TypeSpecificationName}.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private readonly record struct PrimitiveTypeSpecificationInfo
    {
        public PrimitiveTypeSpecificationInfo(string? typeName, string typeAlias, string enumName,
            bool isBuiltInUnmanagedType, string? fullyQualifiedSerializerTypeName)
        {
            TypeName = typeName;
            TypeAlias = typeAlias;
            EnumName = enumName;
            IsBuiltInUnmanagedType = isBuiltInUnmanagedType;
            FullyQualifiedSerializerTypeName = string.IsNullOrEmpty(fullyQualifiedSerializerTypeName)
                ? null
                : $"global::{fullyQualifiedSerializerTypeName}";
        }

        public string EnumName { get; }
        public string? TypeName { get; }
        public string TypeAlias { get; }
        public bool IsBuiltInUnmanagedType { get; }
        public string? FullyQualifiedSerializerTypeName { get; }

        public string SerializerName => FullyQualifiedSerializerTypeName ?? $"{TypeName ?? EnumName}Serializer";
        public string TypeSpecificationName => $"{TypeName ?? EnumName}TypeSpecification";
    }
}
