using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence.</returns>
        [Pure]
        public static IEnumerable<ValueWithLast<TSource>> WithLast<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithLast),
                _ => source.WithLastImplementation(),
            };

        private static Func<TSource, int, ValueWithLast<TSource>> ValueWithLast<TSource>(IList<TSource> list)
            => (value, index)
                => new(value, index == list.Count - 1);

        private static IEnumerable<ValueWithLast<TSource>> WithLastImplementation<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return new ValueWithLast<TSource>(current, false);
                current = enumerator.Current;
            }

            yield return new ValueWithLast<TSource>(current, true);
        }
    }
}
