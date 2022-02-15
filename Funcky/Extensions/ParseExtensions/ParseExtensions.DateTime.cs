using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles style);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles style);
#endif

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

#if DATE_ONLY_TIME_ONLY_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
        public static partial Option<DateOnly> ParseDateOnlyOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
        public static partial Option<DateOnly> ParseDateOnlyOrNone(this string candidate, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
        public static partial Option<DateOnly> ParseDateOnlyOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
        public static partial Option<DateOnly> ParseDateOnlyOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
        public static partial Option<TimeOnly> ParseTimeOnlyOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
        public static partial Option<TimeOnly> ParseTimeOnlyOrNone(this string candidate, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
        public static partial Option<TimeOnly> ParseTimeOnlyOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
        public static partial Option<TimeOnly> ParseTimeOnlyOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles style);
#endif
    }
}
