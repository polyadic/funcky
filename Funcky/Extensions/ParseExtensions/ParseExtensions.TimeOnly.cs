#if TIME_ONLY_SUPPORTED
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
[OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
public static partial class ParseExtensions;
#endif
