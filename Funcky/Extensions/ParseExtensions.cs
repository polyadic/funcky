using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class ParseExtensions
    {
        [Pure]
        public static Option<bool> TryParseBoolean(this string candidate)
            => bool.TryParse(candidate, out var boolResult)
                ? boolResult
                : Option<bool>.None();

        [Pure]
        public static Option<int> TryParseInt(this string candidate)
            => int.TryParse(candidate, out var integerResult)
                ? integerResult
                : Option<int>.None();

        [Pure]
        public static Option<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
            => int.TryParse(candidate, styles, provider, out var integerResult)
                ? integerResult
                : Option<int>.None();

        [Pure]
        public static Option<byte> TryParseByte(this string candidate)
            => byte.TryParse(candidate, out var byteResult)
                ? byteResult
                : Option<byte>.None();

        [Pure]
        public static Option<byte> TryParseByte(this string candidate, NumberStyles styles, IFormatProvider provider)
            => byte.TryParse(candidate, styles, provider, out var byteResult)
                ? byteResult
                : Option<byte>.None();

        [Pure]
        public static Option<short> TryParseShort(this string candidate)
            => short.TryParse(candidate, out var shortResult)
                ? shortResult
                : Option<short>.None();

        [Pure]
        public static Option<short> TryParseShort(this string candidate, NumberStyles styles, IFormatProvider provider)
            => short.TryParse(candidate, styles, provider, out var shortResult)
                ? shortResult
                : Option<short>.None();

        [Pure]
        public static Option<long> TryParseLong(this string candidate)
            => long.TryParse(candidate, out var longResult)
                ? longResult
                : Option<long>.None();

        [Pure]
        public static Option<long> TryParseLong(this string candidate, NumberStyles styles, IFormatProvider provider)
            => long.TryParse(candidate, styles, provider, out var longResult)
                ? longResult
                : Option<long>.None();

        [Pure]
        public static Option<double> TryParseDouble(this string candidate)
            => double.TryParse(candidate, out var doubleResult)
                ? doubleResult
                : Option<double>.None();

        [Pure]
        public static Option<double> TryParseDouble(this string candidate, NumberStyles styles, IFormatProvider provider)
            => double.TryParse(candidate, styles, provider, out var doubleResult)
                ? doubleResult
                : Option<double>.None();

        [Pure]
        public static Option<decimal> TryParseDecimal(this string candidate)
            => decimal.TryParse(candidate, out var decimalResult)
                ? decimalResult
                : Option<decimal>.None();

        [Pure]
        public static Option<decimal> TryParseDecimal(this string candidate, NumberStyles styles, IFormatProvider provider)
            => decimal.TryParse(candidate, styles, provider, out var decimalResult)
                ? decimalResult
                : Option<decimal>.None();

        [Pure]
        public static Option<DateTime> TryParseDateTime(this string candidate)
            => DateTime.TryParse(candidate, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

        [Pure]
        public static Option<DateTime> TryParseDateTime(this string candidate, IFormatProvider provider, DateTimeStyles styles)
            => DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? dateTime
                : Option<DateTime>.None();

        [Pure]
        public static Option<TimeSpan> TryParseTimeSpan(this string candidate)
            => TimeSpan.TryParse(candidate, out var timeSpan)
                ? timeSpan
                : Option<TimeSpan>.None();

        [Pure]
        public static Option<TimeSpan> TryParseTimeSpan(this string candidate, IFormatProvider provider)
            => TimeSpan.TryParse(candidate, provider, out var timeSpan)
                ? timeSpan
                : Option<TimeSpan>.None();

        [Pure]
        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate)
            where TEnum : struct
            => Enum.TryParse(candidate, out TEnum enumValue)
                ? enumValue
                : Option<TEnum>.None();

        [Pure]
        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
            => Enum.TryParse(candidate, ignoreCase, out TEnum enumValue)
                ? enumValue
                : Option<TEnum>.None();
    }
}
