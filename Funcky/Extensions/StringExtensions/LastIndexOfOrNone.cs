using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, char value)
            => MapIndexToOption(haystack.LastIndexOf(value));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value)
            => MapIndexToOption(haystack.LastIndexOf(value));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, comparisonType));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, comparisonType));

        [Pure]
        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count, comparisonType));
    }
}
