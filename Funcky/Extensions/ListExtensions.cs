using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static class ListExtensions
{
    [Pure]
    public static Option<int> IndexOfOrNone<TValue>(this IList<TValue> list, TValue value)
        => MapNotFoundToNone(list.IndexOf(value));

    [Pure]
    public static Option<int> FindIndexOrNone<TValue>(this List<TValue> list, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindIndex(match));

    [Pure]
    public static Option<int> FindIndexOrNone<TValue>(this List<TValue> list, int startIndex, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindIndex(startIndex, match));

    [Pure]
    public static Option<int> FindIndexOrNone<TValue>(this List<TValue> list, int startIndex, int count, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindIndex(startIndex, count, match));

    [Pure]
    public static Option<int> FindLastIndexOrNone<TValue>(this List<TValue> list, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindLastIndex(match));

    [Pure]
    public static Option<int> FindLastIndexOrNone<TValue>(this List<TValue> list, int startIndex, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindLastIndex(startIndex, match));

    [Pure]
    public static Option<int> FindLastIndexOrNone<TValue>(this List<TValue> list, int startIndex, int count, Predicate<TValue> match)
        => MapNotFoundToNone(list.FindLastIndex(startIndex, count, match));
}
