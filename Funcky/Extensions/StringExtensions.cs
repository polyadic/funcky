using System;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class StringExtensions
    {
        public static Option<int> IndexOfOrNone(this string haystack, char value)
            => MapIndexToOption(haystack.IndexOf(value));

        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex)
            => MapIndexToOption(haystack.IndexOf(value, startIndex));

        #if NETSTANDARD2_1
        public static Option<int> IndexOfOrNone(this string haystack, char value, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, comparisonType));
        #endif

        public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count));

        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.IndexOfAny(anyOf));

        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOfAny(anyOf, startIndex, count));

        public static Option<int> IndexOfOrNone(this string haystack, string value)
            => MapIndexToOption(haystack.IndexOf(value));

        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex)
            => MapIndexToOption(haystack.IndexOf(value, startIndex));

        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count));

        public static Option<int> IndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, comparisonType));

        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, comparisonType));

        public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
            => MapIndexToOption(haystack.IndexOf(value, startIndex, count, comparisonType));

        public static Option<int> LastIndexOfOrNone(this string haystack, char value)
            => MapIndexToOption(haystack.LastIndexOf(value));

        public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex));

        public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count));

        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf));

        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex));

        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex, count));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value)
            => MapIndexToOption(haystack.LastIndexOf(value));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, comparisonType));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, comparisonType));

        public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
            => MapIndexToOption(haystack.LastIndexOf(value, startIndex, count, comparisonType));

        private static Option<int> MapIndexToOption(int index)
        {
            const int sentinelValue = -1;
            return index == sentinelValue ? Option<int>.None() : Option.Some(index);
        }
    }
}
