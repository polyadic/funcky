using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ListExtensions
{
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
