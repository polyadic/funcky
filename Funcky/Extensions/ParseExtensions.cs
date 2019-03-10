using System;

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

        public static Maybe<DateTime> TryParseDate(this string candidate)
        {
            if (DateTime.TryParse(candidate, out DateTime dateTime))
            {
                return new Maybe<DateTime>(dateTime);
            }

            return new Maybe<DateTime>();
        }
    }
}
