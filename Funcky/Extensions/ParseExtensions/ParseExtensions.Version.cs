using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    [Pure]
    [OrNoneFromTryPattern(typeof(Version), nameof(Version.TryParse))]
    public static partial Option<Version> ParseVersionOrNone(this string? candidate);

#if READ_ONLY_SPAN_SUPPORTED
    [Pure]
    [OrNoneFromTryPattern(typeof(Version), nameof(Version.TryParse))]
    public static partial Option<Version> ParseVersionOrNone(this ReadOnlySpan<char> candidate);
#endif
}
