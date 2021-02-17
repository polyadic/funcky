using System;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf));

        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex));

        [Pure]
        public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.LastIndexOfAny(anyOf, startIndex, count));
    }
}
