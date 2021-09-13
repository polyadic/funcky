using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        public static Option<bool> ParseBooleanOrNone(this string candidate)
            => FailToOption<bool>.FromTryPattern(bool.TryParse, candidate);

        [Pure]
        public static Option<byte> ParseByteOrNone(this string candidate)
            => FailToOption<byte>.FromTryPattern(byte.TryParse, candidate);

        [Pure]
        public static Option<byte> ParseByteOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<byte>.FromTryPattern(byte.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<short> ParseShortOrNone(this string candidate)
            => FailToOption<short>.FromTryPattern(short.TryParse, candidate);

        [Pure]
        public static Option<short> ParseShortOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<short>.FromTryPattern(short.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<int> ParseIntOrNone(this string candidate)
            => FailToOption<int>.FromTryPattern(int.TryParse, candidate);

        [Pure]
        public static Option<int> ParseIntOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<int>.FromTryPattern(int.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<long> ParseLongOrNone(this string candidate)
            => FailToOption<long>.FromTryPattern(long.TryParse, candidate);

        [Pure]
        public static Option<long> ParseLongOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<long>.FromTryPattern(long.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<float> ParseFloatOrNone(this string candidate)
            => FailToOption<float>.FromTryPattern(float.TryParse, candidate);

        [Pure]
        public static Option<float> ParseFloatOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<float>.FromTryPattern(float.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<double> ParseDoubleOrNone(this string candidate)
            => FailToOption<double>.FromTryPattern(double.TryParse, candidate);

        [Pure]
        public static Option<double> ParseDoubleOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<double>.FromTryPattern(double.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<decimal> ParseDecimalOrNone(this string candidate)
            => FailToOption<decimal>.FromTryPattern(decimal.TryParse, candidate);

        [Pure]
        public static Option<decimal> ParseDecimalOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<decimal>.FromTryPattern(decimal.TryParse, candidate, styles, provider);

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

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate)
            where TEnum : struct
            => FailToOption<TEnum>.FromTryPattern(Enum.TryParse, candidate);

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
            => FailToOption<TEnum>.FromTryPattern(Enum.TryParse, candidate, ignoreCase);
    }
}
