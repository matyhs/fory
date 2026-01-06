using Fory.Core.Spec;

namespace Fory.Core.Extensions;

internal static class SerializationContextExtensions
{
    public static bool IsPeerXlang(this DeserializationContext context)
    {
        return context.HeaderBitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsXlang);
    }

    public static bool IsPeerLittleEdian(this DeserializationContext context)
    {
        return context.HeaderBitmap.HasFlag(ForyHeaderSpec.ForyConfigFlags.IsLittleEdian);
    }
}
