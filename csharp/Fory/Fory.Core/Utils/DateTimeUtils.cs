using System;

namespace Fory.Core.Utils;

internal static class DateTimeUtils
{
#if NETSTANDARD
    private static readonly DateTimeOffset UnixEpoch = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
#endif

    public static DateTimeOffset GetUnixEpoch()
    {
#if NETSTANDARD
        return UnixEpoch;
#else
        return DateTimeOffset.UnixEpoch;
#endif
    }
}
