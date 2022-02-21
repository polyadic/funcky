#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
#if DATE_ONLY_SUPPORTED
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
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this string candidate, string format);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this string candidate, string[] formats);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this string candidate, string format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this string candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this ReadOnlySpan<char> candidate, string[] formats);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider provider, DateTimeStyles style);

        [Pure]
        [OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
        public static partial Option<DateOnly> ParseExactDateOnlyOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);

#endif
    }
}
