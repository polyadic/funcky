using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    [Pure]
    public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf)
        => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf));

    [Pure]
    public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex)
        => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf, startIndex));

    [Pure]
    public static Option<int> LastIndexOfAnyOrNone(this string haystack, char[] anyOf, int startIndex, int count)
        => MapNotFoundToNone(haystack.LastIndexOfAny(anyOf, startIndex, count));
}
