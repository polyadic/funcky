using System.Net;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(IPAddress), nameof(IPAddress.TryParse))]
        public static partial Option<IPAddress> ParseIPAddressOrNone(string? candidate);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(IPAddress), nameof(IPAddress.TryParse))]
        public static partial Option<IPAddress> ParseIPAddressOrNone(ReadOnlySpan<char> candidate);
#endif

#if IP_END_POINT_TRY_PARSE_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(IPEndPoint), nameof(IPEndPoint.TryParse))]
        public static partial Option<IPEndPoint> TryParseIPEndPointOrNone(string candidate);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(IPEndPoint), nameof(IPEndPoint.TryParse))]
        public static partial Option<IPEndPoint> TryParseIPEndPointOrNone(ReadOnlySpan<char> candidate);
#endif
#endif
    }
}
