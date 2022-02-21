using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this string candidate, IFormatProvider provider);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider);
#endif

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this string candidate, string format, IFormatProvider formatProvider);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this string candidate, string format, IFormatProvider formatProvider, TimeSpanStyles styles);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this string candidate, string[] formats, IFormatProvider formatProvider);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this string candidate, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider formatProvider);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider formatProvider, TimeSpanStyles styles);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider formatProvider);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
        public static partial Option<TimeSpan> ParseExactTimeSpanOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles);
#endif
    }
}
