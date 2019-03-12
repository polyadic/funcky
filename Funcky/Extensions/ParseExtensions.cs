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
                : new Option<bool>();
        }

        public static Option<int> TryParseInt(this string candidate)
        {
            return int.TryParse(candidate, out var integerResult)
                ? new Option<int>(integerResult)
                : new Option<int>();
        }

        public static Option<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return int.TryParse(candidate, styles, provider, out var integerResult)
                ? new Option<int>(integerResult)
                : new Option<int>();
        }

        public static Option<byte> TryParseByte(this string candidate)
        {
            return byte.TryParse(candidate, out var byteResult)
                ? new Option<byte>(byteResult)
                : new Option<byte>();
        }

        public static Option<byte> TryParseByte(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return byte.TryParse(candidate, styles, provider, out var byteResult)
                ? new Option<byte>(byteResult)
                : new Option<byte>();
        }

        public static Option<short> TryParseShort(this string candidate)
        {
            return short.TryParse(candidate, out var shortResult)
                ? new Option<short>(shortResult)
                : new Option<short>();
        }

        public static Option<short> TryParseShort(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return short.TryParse(candidate, styles, provider, out var shortResult)
                ? new Option<short>(shortResult)
                : new Option<short>();
        }

        public static Option<long> TryParseLong(this string candidate)
        {
            return long.TryParse(candidate, out var longResult)
                ? new Option<long>(longResult)
                : new Option<long>();
        }

        public static Option<long> TryParseLong(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return long.TryParse(candidate, styles, provider, out var longResult)
                ? new Option<long>(longResult)
                : new Option<long>();
        }

        public static Option<double> TryParseDouble(this string candidate)
        {
            return double.TryParse(candidate, out var doubleResult)
                ? new Option<double>(doubleResult)
                : new Option<double>();
        }

        public static Option<double> TryParseDouble(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return double.TryParse(candidate, styles, provider, out var doubleResult)
                ? new Option<double>(doubleResult)
                : new Option<double>();
        }

        public static Option<decimal> TryParseDecimal(this string candidate)
        {
            return decimal.TryParse(candidate, out var decimalResult)
                ? new Option<decimal>(decimalResult)
                : new Option<decimal>();
        }

        public static Option<decimal> TryParseDecimal(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return decimal.TryParse(candidate, styles, provider, out var decimalResult)
                ? new Option<decimal>(decimalResult)
                : new Option<decimal>();
        }

        public static Option<DateTime> TryParseDateTime(this string candidate)
        {
            return DateTime.TryParse(candidate, out var dateTime)
                ? new Option<DateTime>(dateTime)
                : new Option<DateTime>();
        }

        public static Option<DateTime> TryParseDateTime(this string candidate, IFormatProvider provider, DateTimeStyles styles)
        {
            return DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? new Option<DateTime>(dateTime)
                : new Option<DateTime>();
        }

        public static Option<TimeSpan> TryParseTimeSpan(this string candidate)
        {
            return TimeSpan.TryParse(candidate, out var timeSpan)
                ? new Option<TimeSpan>(timeSpan)
                : new Option<TimeSpan>();
        }

        public static Option<TimeSpan> TryParseTimeSpan(this string candidate, IFormatProvider provider)
        {
            return TimeSpan.TryParse(candidate, provider, out var timeSpan)
                ? new Option<TimeSpan>(timeSpan)
                : new Option<TimeSpan>();
        }

        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate) where TEnum : struct
        {
            return Enum.TryParse(candidate, out TEnum enumValue)
                ? new Option<TEnum>(enumValue)
                : new Option<TEnum>();
        }

        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase) where TEnum : struct
        {
            return Enum.TryParse(candidate, ignoreCase, out TEnum enumValue)
                ? new Option<TEnum>(enumValue)
                : new Option<TEnum>();
        }
    }
}
