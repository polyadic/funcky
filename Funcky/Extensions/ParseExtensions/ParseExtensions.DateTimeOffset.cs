using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParse))]
        public static partial Option<DateTimeOffset> ParseDateTimeOffsetOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParse))]
        public static partial Option<DateTimeOffset> ParseDateTimeOffsetOrNone(this string candidate, IFormatProvider provider, DateTimeStyles style);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParse))]
        public static partial Option<DateTimeOffset> ParseDateTimeOffsetOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParse))]
        public static partial Option<DateTimeOffset> ParseDateTimeOffsetOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles style);
#endif

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParseExact))]
        public static partial Option<DateTimeOffset> ParseExactDateTimeOffsetOrNone(this string candidate, string format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParseExact))]
        public static partial Option<DateTimeOffset> ParseExactDateTimeOffsetOrNone(this string candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParseExact))]
        public static partial Option<DateTimeOffset> ParseExactDateTimeOffsetOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParseExact))]
        public static partial Option<DateTimeOffset> ParseExactDateTimeOffsetOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);
#endif
    }
}
