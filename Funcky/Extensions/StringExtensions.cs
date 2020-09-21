using System;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class StringExtensions
    {
        private const int NotFoundValue = -1;

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value)
            => MapIndexToOption(haystack.IndexOf(value));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex)
            => MapIndexToOption(haystack.IndexOf(value, startIndex));

        [Pure]
        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count));

#if NETSTANDARD2_1 || NETCOREAPP3_1
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

        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.IndexOfAny(anyOf));

        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
            => MapIndexToOption(haystack.IndexOfAny(anyOf, startIndex));

        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOfAny(anyOf, startIndex, count));

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

        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf));

        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex));

        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex, count));

        [Pure]
        private static Option<int> MapIndexToOption(int index)
            => index == NotFoundValue
                   ? Option<int>.None()
                   : Option.Some(index);
    }
}
