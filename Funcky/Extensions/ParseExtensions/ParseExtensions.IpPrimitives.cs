using System.Net;
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(IPAddress), nameof(IPAddress.TryParse))]
#if IP_END_POINT_TRY_PARSE_SUPPORTED
[OrNoneFromTryPattern(typeof(IPEndPoint), nameof(IPEndPoint.TryParse))]
#endif
public static partial class ParseExtensions;
