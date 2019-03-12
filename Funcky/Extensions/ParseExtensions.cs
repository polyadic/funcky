using System;
using System.Globalization;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class ParseExtensions
    {
        public static Maybe<bool> TryParseBoolean(this string candidate)
        {
            return bool.TryParse(candidate, out var boolResult)
                ? new Maybe<bool>(boolResult)
                : new Maybe<bool>();
        }

        public static Maybe<int> TryParseInt(this string candidate)
        {
            return int.TryParse(candidate, out var integerResult)
                ? new Maybe<int>(integerResult)
                : new Maybe<int>();
        }

        public static Maybe<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return int.TryParse(candidate, styles, provider, out var integerResult)
                ? new Maybe<int>(integerResult)
                : new Maybe<int>();
        }

        public static Maybe<byte> TryParseByte(this string candidate)
        {
            return byte.TryParse(candidate, out var byteResult)
                ? new Maybe<byte>(byteResult)
                : new Maybe<byte>();
        }

        public static Maybe<byte> TryParseByte(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return byte.TryParse(candidate, styles, provider, out var byteResult)
                ? new Maybe<byte>(byteResult)
                : new Maybe<byte>();
        }

        public static Maybe<short> TryParseShort(this string candidate)
        {
            return short.TryParse(candidate, out var shortResult)
                ? new Maybe<short>(shortResult)
                : new Maybe<short>();
        }

        public static Maybe<short> TryParseShort(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return short.TryParse(candidate, styles, provider, out var shortResult)
                ? new Maybe<short>(shortResult)
                : new Maybe<short>();
        }

        public static Maybe<long> TryParseLong(this string candidate)
        {
            return long.TryParse(candidate, out var longResult)
                ? new Maybe<long>(longResult)
                : new Maybe<long>();
        }

        public static Maybe<long> TryParseLong(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return long.TryParse(candidate, styles, provider, out var longResult)
                ? new Maybe<long>(longResult)
                : new Maybe<long>();
        }

        public static Maybe<double> TryParseDouble(this string candidate)
        {
            return double.TryParse(candidate, out var doubleResult)
                ? new Maybe<double>(doubleResult)
                : new Maybe<double>();
        }

        public static Maybe<double> TryParseDouble(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return double.TryParse(candidate, styles, provider, out var doubleResult)
                ? new Maybe<double>(doubleResult)
                : new Maybe<double>();
        }

        public static Maybe<decimal> TryParseDecimal(this string candidate)
        {
            return decimal.TryParse(candidate, out var decimalResult)
                ? new Maybe<decimal>(decimalResult)
                : new Maybe<decimal>();
        }

        public static Maybe<decimal> TryParseDecimal(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            return decimal.TryParse(candidate, styles, provider, out var decimalResult)
                ? new Maybe<decimal>(decimalResult)
                : new Maybe<decimal>();
        }

        public static Maybe<DateTime> TryParseDateTime(this string candidate)
        {
            return DateTime.TryParse(candidate, out var dateTime)
                ? new Maybe<DateTime>(dateTime)
                : new Maybe<DateTime>();
        }

        public static Maybe<DateTime> TryParseDateTime(this string candidate, IFormatProvider provider, DateTimeStyles styles)
        {
            return DateTime.TryParse(candidate, provider, styles, out var dateTime)
                ? new Maybe<DateTime>(dateTime)
                : new Maybe<DateTime>();
        }

        public static Maybe<TimeSpan> TryParseTimeSpan(this string candidate)
        {
            return TimeSpan.TryParse(candidate, out var timeSpan)
                ? new Maybe<TimeSpan>(timeSpan)
                : new Maybe<TimeSpan>();
        }

        public static Maybe<TimeSpan> TryParseTimeSpan(this string candidate, IFormatProvider provider)
        {
            return TimeSpan.TryParse(candidate, provider, out var timeSpan)
                ? new Maybe<TimeSpan>(timeSpan)
                : new Maybe<TimeSpan>();
        }

        public static Maybe<TEnum> TryParseEnum<TEnum>(this string candidate) where TEnum : struct
        {
            return Enum.TryParse(candidate, out TEnum enumValue)
                ? new Maybe<TEnum>(enumValue)
                : new Maybe<TEnum>();
        }

        public static Maybe<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase) where TEnum : struct
        {
            return Enum.TryParse(candidate, ignoreCase, out TEnum enumValue)
                ? new Maybe<TEnum>(enumValue)
                : new Maybe<TEnum>();
        }
    }
}
