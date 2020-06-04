using System;
using System.Globalization;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class ParseExtensions
    {
        public static Option<bool> TryParseBoolean(this string candidate)
        {
            return bool.TryParse(candidate, out var boolResult)
                ? new Option<bool>(boolResult)
                : Option<bool>.None();
        }

        public static Option<int> TryParseInt(this string candidate)
        {
            return int.TryParse(candidate, out var integerResult)
                ? new Option<int>(integerResult)
                : Option<int>.None();
        }

        public static Option<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return int.TryParse(candidate, styles, provider, out var integerResult)
                ? new Option<int>(integerResult)
                : Option<int>.None();
        }

        public static Option<byte> TryParseByte(this string candidate)
        {
            return byte.TryParse(candidate, out var byteResult)
                ? new Option<byte>(byteResult)
                : Option<byte>.None();
        }

        public static Option<byte> TryParseByte(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return byte.TryParse(candidate, styles, provider, out var byteResult)
                ? new Option<byte>(byteResult)
                : Option<byte>.None();
        }

        public static Option<short> TryParseShort(this string candidate)
        {
            return short.TryParse(candidate, out var shortResult)
                ? new Option<short>(shortResult)
                : Option<short>.None();
        }

        public static Option<short> TryParseShort(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return short.TryParse(candidate, styles, provider, out var shortResult)
                ? new Option<short>(shortResult)
                : Option<short>.None();
        }

        public static Option<long> TryParseLong(this string candidate)
        {
            return long.TryParse(candidate, out var longResult)
                ? new Option<long>(longResult)
                : Option<long>.None();
        }

        public static Option<long> TryParseLong(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return long.TryParse(candidate, styles, provider, out var longResult)
                ? new Option<long>(longResult)
                : Option<long>.None();
        }

        public static Option<double> TryParseDouble(this string candidate)
        {
            return double.TryParse(candidate, out var doubleResult)
                ? new Option<double>(doubleResult)
                : Option<double>.None();
        }

        public static Option<double> TryParseDouble(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return double.TryParse(candidate, styles, provider, out var doubleResult)
                ? new Option<double>(doubleResult)
                : Option<double>.None();
        }

        public static Option<decimal> TryParseDecimal(this string candidate)
        {
            return decimal.TryParse(candidate, out var decimalResult)
                ? new Option<decimal>(decimalResult)
                : Option<decimal>.None();
        }

        public static Option<decimal> TryParseDecimal(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return decimal.TryParse(candidate, styles, provider, out var decimalResult)
                ? new Option<decimal>(decimalResult)
                : Option<decimal>.None();
        }

        public static Option<DateTime> TryParseDateTime(this string candidate)
        {
            return DateTime.TryParse(candidate, out var dateTime)
                ? new Option<DateTime>(dateTime)
                : Option<DateTime>.None();
        }

        public static Option<DateTime> TryParseDateTime(this string candidate, IFormatProvider provider, DateTimeStyles styles)
        {
            return DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? new Option<DateTime>(dateTime)
                : Option<DateTime>.None();
        }

        public static Option<TimeSpan> TryParseTimeSpan(this string candidate)
        {
            return TimeSpan.TryParse(candidate, out var timeSpan)
                ? new Option<TimeSpan>(timeSpan)
                : Option<TimeSpan>.None();
        }

        public static Option<TimeSpan> TryParseTimeSpan(this string candidate, IFormatProvider provider)
        {
            return TimeSpan.TryParse(candidate, provider, out var timeSpan)
                ? new Option<TimeSpan>(timeSpan)
                : Option<TimeSpan>.None();
        }

        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate)
            where TEnum : struct
        {
            return Enum.TryParse(candidate, out TEnum enumValue)
                ? new Option<TEnum>(enumValue)
                : Option<TEnum>.None();
        }

        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
        {
            return Enum.TryParse(candidate, ignoreCase, out TEnum enumValue)
                ? new Option<TEnum>(enumValue)
                : Option<TEnum>.None();
        }
    }
}
