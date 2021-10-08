using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this string candidate)
            => FailToOption<DateTime>.FromTryPattern(DateTime.TryParse, candidate);

        [Pure]
        public static Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles)
            => FailToOption<DateTime>.FromTryPattern(DateTime.TryParse, candidate, provider, styles);

        [Pure]
        public static Option<TimeSpan> ParseTimeSpanOrNone(this string candidate)
            => FailToOption<TimeSpan>.FromTryPattern(TimeSpan.TryParse, candidate);

        [Pure]
        public static Option<TimeSpan> ParseTimeSpanOrNone(this string candidate, IFormatProvider provider)
            => FailToOption<TimeSpan>.FromTryPattern(TimeSpan.TryParse, candidate, provider);
    }
}
