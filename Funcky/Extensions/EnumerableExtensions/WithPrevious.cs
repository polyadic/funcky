using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
        /// <exception cref="ArgumentNullException">Thrown when any value in <paramref name="source"/> is <see langword="null"/>.</exception>
        [Pure]
        public static IEnumerable<ValueWithPrevious<TSource>> WithPrevious<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithPrevious),
                _ => source.WithPreviousImplementation(),
            };

        private static Func<TSource, int, ValueWithPrevious<TSource>> ValueWithPrevious<TSource>(IList<TSource> list)
            where TSource : notnull
            => (value, index)
                => new(value, list.ElementAtOrNone(IndexOfPrevious(index)));

        private static int IndexOfPrevious(int index)
            => index - 1;

        private static IEnumerable<ValueWithPrevious<TSource>> WithPreviousImplementation<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
        {
            var previous = Option<TSource>.None();

            foreach (var value in source)
            {
                yield return new ValueWithPrevious<TSource>(value, previous);
                previous = value;
            }
        }
    }
}
