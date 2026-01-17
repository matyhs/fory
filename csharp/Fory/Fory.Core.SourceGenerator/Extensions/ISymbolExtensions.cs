using Microsoft.CodeAnalysis;

namespace Fory.Core.SourceGenerator.Extensions;

internal static class ISymbolExtensions
{
    public static bool IsBuiltInUnmanagedType(this ITypeSymbol symbol) => symbol.SpecialType switch
    {
        SpecialType.System_SByte or
        SpecialType.System_Byte or
        SpecialType.System_Int16 or
        SpecialType.System_UInt16 or
        SpecialType.System_Int32 or
        SpecialType.System_UInt32 or
        SpecialType.System_Int64 or
        SpecialType.System_UInt64 or
        SpecialType.System_Char or
        SpecialType.System_Single or
        SpecialType.System_Double or
        SpecialType.System_Decimal or
        SpecialType.System_Boolean => true,
        _ => false
    };
}
