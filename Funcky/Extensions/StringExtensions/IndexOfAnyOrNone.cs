using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    extension(string haystack)
    {
        [Pure]
        public Option<int> IndexOfAnyOrNone(char[] anyOf)
            => MapNotFoundToNone(haystack.IndexOfAny(anyOf));

        [Pure]
        public Option<int> IndexOfAnyOrNone(char[] anyOf, int startIndex)
            => MapNotFoundToNone(haystack.IndexOfAny(anyOf, startIndex));

        [Pure]
        public Option<int> IndexOfAnyOrNone(char[] anyOf, int startIndex, int count)
            => MapNotFoundToNone(haystack.IndexOfAny(anyOf, startIndex, count));
    }
}
