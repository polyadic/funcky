using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, char value)
        => MapNotFoundToNone(haystack.IndexOf(value));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, char value, int startIndex, int count)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, char value, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.IndexOf(value, comparisonType));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value)
        => MapNotFoundToNone(haystack.IndexOf(value));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.IndexOf(value, comparisonType));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex, comparisonType));

    [Pure]
    public static Option<int> IndexOfOrNone(this string haystack, string value, int startIndex, int count, StringComparison comparisonType)
        => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count, comparisonType));
}
