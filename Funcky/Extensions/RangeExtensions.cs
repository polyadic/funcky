#if RANGE_SUPPORTED
using System.Linq;
using Funcky.Internal;

namespace Funcky.Extensions
{
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
        public static IEnumerable<TResult> SelectMany<TItem, TResult>(this Range source, Func<int, IEnumerable<TItem>> collectionSelector, Func<int, TItem, TResult> resultSelector)
            => source.ToRangeEnumerable().SelectMany(collectionSelector, resultSelector);

        [Pure]
        public static IEnumerable<TResult> SelectMany<TResult>(this Range source, Func<int, Range> rangeSelector, Func<int, int, TResult> resultSelector)
            => source.ToRangeEnumerable().SelectMany(TransformSelector(rangeSelector), resultSelector);

        [Pure]
        public static IEnumerable<TResult> SelectMany<TItem, TResult>(this IEnumerable<TItem> source, Func<TItem, Range> rangeSelector, Func<TItem, int, TResult> resultSelector)
            => source.SelectMany(TransformSelector(rangeSelector), resultSelector);

        private static IEnumerator<int> IterateRange(int start, int end)
        {
            for (var index = start; CanAdvance(index, end); index = Advance(index, GetDirection(start, end)))
            {
                yield return index;
            }
        }

        private static RangeEnumerable ToRangeEnumerable(this Range source)
            => new(source);

        private static Func<TItem, RangeEnumerable> TransformSelector<TItem>(Func<TItem, Range> rangeSelector)
            => value
                => rangeSelector(value).ToRangeEnumerable();

        private static int Advance(int index, int direction)
            => index + direction;

        private static bool CanAdvance(int index, int end)
            => index != end;

        private static int GetDirection(int start, int end)
            => end.CompareTo(start);

        private static int ToInt(Index index)
            => index.IsFromEnd
                ? -index.Value
                : index.Value;
    }
}
#endif
