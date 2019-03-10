using System;

namespace Funcky
{
    public static class TryParseHelper
    {
        public static Maybe<int> TryParseInt(string candidate)
        {
            if (int.TryParse(candidate, out int integerResult))
            {
                return new Maybe<int>(integerResult);
            }

            return new Maybe<int>();
        }

        public static Maybe<DateTime> TryParseDate(string candidate)
        {
            if (DateTime.TryParse(candidate, out DateTime dateTime))
            {
                return new Maybe<DateTime>(dateTime);
            }

            return new Maybe<DateTime>();
        }
    }
}
