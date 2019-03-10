using System;
using System.Globalization;

namespace Funcky.Extensions
{
    public static class ParseExtensions
    {
        public static Maybe<int> TryParseInt(this string candidate)
        {
            if (int.TryParse(candidate, out int integerResult))
            {
                return new Maybe<int>(integerResult);
            }

            return new Maybe<int>();
        }

        public static Maybe<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
        {
            if (int.TryParse(candidate, styles, provider, out int integerResult))
            {
                return new Maybe<int>(integerResult);
            }

            return new Maybe<int>();
        }

        public static Maybe<DateTime> TryParseDate(this string candidate)
        {
            if (DateTime.TryParse(candidate, out DateTime dateTime))
            {
                return new Maybe<DateTime>(dateTime);
            }

            return new Maybe<DateTime>();
        }

        public static Maybe<DateTime> TryParseDate(this string candidate, IFormatProvider provider, DateTimeStyles styles)
        {
            if (DateTime.TryParse(candidate, provider, styles, out DateTime dateTime))
            {
                return new Maybe<DateTime>(dateTime);
            }

            return new Maybe<DateTime>();
        }

        public static Maybe<TEnum> TryParseEnum<TEnum>(this string candidate) where TEnum : struct
        {
            if (Enum.TryParse(candidate, out TEnum enumValue))
            {
                return new Maybe<TEnum>(enumValue);
            }

            return new Maybe<TEnum>();
        }

        public static Maybe<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase) where TEnum : struct
        {
            if (Enum.TryParse(candidate, ignoreCase, out TEnum enumValue))
            {
                return new Maybe<TEnum>(enumValue);
            }

            return new Maybe<TEnum>();
        }
    }
}
