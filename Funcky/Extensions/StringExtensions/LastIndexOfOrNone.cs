using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, char value)
        => MapNotFoundToNone(haystack.LastIndexOf(value));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, char value, int startIndex, int count)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value)
        => MapNotFoundToNone(haystack.LastIndexOf(value));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.LastIndexOf(value, comparisonType));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, comparisonType));

    [Pure]
    public static Option<int> LastIndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count, comparisonType));
}
