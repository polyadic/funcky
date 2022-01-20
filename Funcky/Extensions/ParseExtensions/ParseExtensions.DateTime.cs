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
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
        public static partial Option<TimeSpan> ParseTimeSpanOrNone(this string candidate, IFormatProvider provider);

#if NET6_0_OR_GREATER
        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
        public static partial Option<DateOnly> ParseDateOnlyOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParse))]
        public static partial Option<TimeOnly> ParseTimeOnlyOrNone(this string candidate);
#endif
    }
}
