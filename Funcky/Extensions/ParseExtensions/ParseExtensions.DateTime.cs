using System.Globalization;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this string candidate)
            => DateTime.TryParse(candidate, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate)
            => DateTime.TryParse(candidate, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();
#endif

        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles)
            => DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this ReadOnlySpan<char> candidate, IFormatProvider provider, DateTimeStyles styles)
            => DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();
#endif

        [Pure]
        public static Option<DateTime> ParseExactDateTimeOrNone(this string? candidate, string? format, IFormatProvider? provider, DateTimeStyles style)
            => DateTime.TryParseExact(candidate, format, provider, style, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        public static Option<DateTime> ParseExactDateTimeOrNone(this ReadOnlySpan<char> candidate, ReadOnlySpan<char> format, IFormatProvider? provider, DateTimeStyles style)
            => DateTime.TryParseExact(candidate, format, provider, style, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();
#endif

        [Pure]
        public static Option<DateTime> ParseExactDateTimeOrNone(this string? candidate, string?[]? formats, IFormatProvider? provider, DateTimeStyles style)
            => DateTime.TryParseExact(candidate, formats, provider, style, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        public static Option<DateTime> ParseExactDateTimeOrNone(this ReadOnlySpan<char> candidate, string?[]? formats, IFormatProvider? provider, DateTimeStyles style)
            => DateTime.TryParseExact(candidate, formats, provider, style, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();
#endif

        [Pure]
        public static Option<TimeSpan> ParseTimeSpanOrNone(this string candidate)
            => TimeSpan.TryParse(candidate, out var timeSpan)
                ? timeSpan
                : Option<TimeSpan>.None();

        [Pure]
        public static Option<TimeSpan> ParseTimeSpanOrNone(this string candidate, IFormatProvider provider)
            => TimeSpan.TryParse(candidate, provider, out var timeSpan)
                ? timeSpan
                : Option<TimeSpan>.None();
    }
}
