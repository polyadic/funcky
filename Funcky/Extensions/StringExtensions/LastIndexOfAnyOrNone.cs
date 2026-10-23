using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    extension(string haystack)
    {
        [Pure]
        public Option<int> LastIndexOfAnyOrNone(char[] anyOf)
            => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf));

        [Pure]
        public Option<int> LastIndexOfAnyOrNone(char[] anyOf, int startIndex)
            => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf, startIndex));

        [Pure]
        public Option<int> LastIndexOfAnyOrNone(char[] anyOf, int startIndex, int count)
            => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf, startIndex, count));
    }
}
