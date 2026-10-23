#if RANGE_SUPPORTED
using Funcky.Internal;

namespace Funcky.Extensions;

public static class RangeExtensions
{
    extension(Range range)
    {
        public IEnumerator<int> GetEnumerator()
            => IterateRange(ToInt(range.Start), ToInt(range.End));
    }

    extension(Range source)
    {
        [Pure]
        public IEnumerable<TResult> Select<TResult>(Func<int, TResult> selector)
            => source.ToRangeEnumerable().Select(selector);

        [Pure]
        public IEnumerable<TResult> SelectMany<TResult>(Func<int, IEnumerable<TResult>> selector)
            => source.ToRangeEnumerable().SelectMany(selector);

        [Pure]
        public IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<int, IEnumerable<TCollection>> selector, Func<int, TCollection, TResult> resultSelector)
            => source.ToRangeEnumerable().SelectMany(selector, resultSelector);

        [Pure]
        public IEnumerable<TResult> SelectMany<TResult>(Func<int, Range> selector, Func<int, int, TResult> resultSelector)
            => source.ToRangeEnumerable().SelectMany(TransformSelector(selector), resultSelector);

        private RangeEnumerable ToRangeEnumerable()
            => new(source);
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        [Pure]
        public IEnumerable<TResult> SelectMany<TResult>(Func<TSource, Range> selector, Func<TSource, int, TResult> resultSelector)
            => source.SelectMany(TransformSelector(selector), resultSelector);
    }

    private static IEnumerator<int> IterateRange(int start, int end)
    {
        for (var index = start; CanAdvance(index, end); index = Advance(index, GetDirection(start, end)))
        {
            yield return index;
        }
    }

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
