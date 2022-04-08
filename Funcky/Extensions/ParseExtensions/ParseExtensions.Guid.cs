using Funcky.Internal;

namespace Funcky.Extensions
{
    public partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParse))]
        public static partial Option<Guid> ParseGuidOrNone(this string? candidate);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParse))]
        public static partial Option<Guid> ParseGuidOrNone(this ReadOnlySpan<char> candidate);
#endif

        [Pure]
        [OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParseExact))]
        public static partial Option<Guid> ParseExactGuidOrNone(this string? candidate, string? format);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParseExact))]
        public static partial Option<Guid> ParseExactGuidOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format);
#endif
    }
}
