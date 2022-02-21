using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate);

        // TODO for funcky3 change parameter styles to style to be consistent with .NET
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
        public static partial Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles style);
#endif

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParseExact))]
        public static partial Option<DateTime> ParseExactDateTimeOrNone(this string candidate, string format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParseExact))]
        public static partial Option<DateTime> ParseExactDateTimeOrNone(this string candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParseExact))]
        public static partial Option<DateTime> ParseExactDateTimeOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParseExact))]
        public static partial Option<DateTime> ParseExactDateTimeOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);
#endif
    }
}
