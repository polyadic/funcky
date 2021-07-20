namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf)
            => MapIndexToOption(haystack.IndexOfAny(anyOf));

        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
            => MapIndexToOption(haystack.IndexOfAny(anyOf, startIndex));

        [Pure]
        public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
            => MapIndexToOption(haystack.IndexOfAny(anyOf, startIndex, count));
    }
}
