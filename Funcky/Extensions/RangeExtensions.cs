#if RANGE_SUPPORTED
using Funcky.Internal;

namespace Funcky.Extensions;

public static class RangeExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range range)
        => IterateRange(ToInt(range.Start), ToInt(range.End));

    [Pure]
    public static IEnumerable<TResult> Select<TResult>(this Range source, Func<int, TResult> selector)
        => source.ToRangeEnumerable().Select(selector);

    [Pure]
    public static IEnumerable<TResult> SelectMany<TResult>(this Range source, Func<int, IEnumerable<TResult>> selector)
        => source.ToRangeEnumerable().SelectMany(selector);

    [Pure]
    public static IEnumerable<TResult> SelectMany<TItem, TResult>(this Range source, Func<int, IEnumerable<TItem>> selector, Func<int, TItem, TResult> resultSelector)
        => source.ToRangeEnumerable().SelectMany(selector, resultSelector);

    [Pure]
    public static IEnumerable<TResult> SelectMany<TResult>(this Range source, Func<int, Range> selector, Func<int, int, TResult> resultSelector)
        => source.ToRangeEnumerable().SelectMany(TransformSelector(selector), resultSelector);

    [Pure]
    public static IEnumerable<TResult> SelectMany<TItem, TResult>(this IEnumerable<TItem> source, Func<TItem, Range> selector, Func<TItem, int, TResult> resultSelector)
        => source.SelectMany(TransformSelector(selector), resultSelector);

    private static IEnumerator<int> IterateRange(int start, int end)
    {
        for (var index = start; CanAdvance(index, end); index = Advance(index, GetDirection(start, end)))
        {
            yield return index;
        }
    }

    private static RangeEnumerable ToRangeEnumerable(this Range source)
        => new(source);

    private static Func<TItem, RangeEnumerable> TransformSelector<TItem>(Func<TItem, Range> selector)
        => value
            => selector(value).ToRangeEnumerable();

    private static int Advance(int index, int direction)
        => index + direction;

    private static bool CanAdvance(int index, int end)
        => index != end;

    private static int GetDirection(int start, int end)
        => end.CompareTo(start);

    private static int ToInt(Index index)
        => index.IsFromEnd
            ? throw new ArgumentException("Index has set 'FromEnd = true': the Funcky range-extensions do not support syntax for negative numbers.", nameof(index))
            : index.Value;
}
#endif
