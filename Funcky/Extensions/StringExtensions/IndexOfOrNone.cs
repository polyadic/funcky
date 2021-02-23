using System;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value)
            => MapIndexToOption(haystack.IndexOf(value));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex)
            => MapIndexToOption(haystack.IndexOf(value, startIndex));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count));

        #if INDEX_OF_CHAR_COMPARISONTYPE_SUPPORTED
        public static Option<int> IndexOfOrNone(this string haystack, char value, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, comparisonType));
        #endif

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value)
            => MapIndexToOption(haystack.IndexOf(value));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex)
            => MapIndexToOption(haystack.IndexOf(value, startIndex));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, comparisonType));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, comparisonType));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count, comparisonType));
    }
}
