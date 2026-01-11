using System.Runtime.Serialization;
using Fory.Core.Spec;

namespace Fory.Core.Extensions;

public static class HeaderInfoExtensions
{
    public static bool IsHeaderValidOrThrow(this HeaderInfo headerInfo, DeserializationContext context)
    {
        if (headerInfo.IsPeerXlang != context.IsXlang)
            throw new SerializationException(
                "Mismatch found in header bitmap between xlang bit and current Fory configuration.");

        if (!headerInfo.IsPeerLittleEdian)
            throw new SerializationException("Big endian is currently not supported");

        return true;
    }
}
