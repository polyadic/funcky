#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
#if TIME_ONLY_SUPPORTED
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

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this string candidate, string format);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this string candidate, string[] formats);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this string candidate, string format, IFormatProvider provider, DateTimeStyles style);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this string candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this ReadOnlySpan<char> candidate, string[] formats);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider provider, DateTimeStyles style);

    [Pure]
    [OrNoneFromTryPattern(typeof(TimeOnly), nameof(TimeOnly.TryParseExact))]
    public static partial Option<TimeOnly> ParseExactTimeOnlyOrNone(this ReadOnlySpan<char> candidate, string[] formats, IFormatProvider provider, DateTimeStyles style);
#endif
}
