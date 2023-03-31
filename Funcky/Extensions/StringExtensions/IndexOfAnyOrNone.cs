using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    [Pure]
    public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf)
        => MapNotFoundToNone(haystack.IndexOfAny(anyOf));

    [Pure]
    public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
        => MapNotFoundToNone(haystack.IndexOfAny(anyOf, startIndex));

    [Pure]
    public static Option<int> IndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
        => MapNotFoundToNone(haystack.IndexOfAny(anyOf, startIndex, count));
}
