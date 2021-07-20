using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// The PowerSet function returns a sequence with the set of all subsets.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Returns an sequence which includes all subsets of the given sequence.</returns>
        /// <remarks>The PowerSet function returns a sequence with 2^n elements where n is the number of elements int the source sequence.
        /// This means it is only viable for small source sequences.</remarks>
        public static IEnumerable<IEnumerable<TSource>> PowerSet<TSource>(this IEnumerable<TSource> source)
            => source.PowerSetInternal();

        private static IEnumerable<IEnumerable<TSource>> PowerSetInternal<TSource>(this IEnumerable<TSource> source)
        {
            using var sourceEnumerator = source.GetEnumerator();

            foreach (var set in PowerSetEnumerator(sourceEnumerator))
            {
                yield return set;
            }
        }

        private static IEnumerable<ImmutableStack<TSource>> PowerSetEnumerator<TSource>(this IEnumerator<TSource> source)
        {
            if (source.MoveNext())
            {
                var temp = source.Current;
                foreach (var set in source.PowerSetEnumerator())
                {
                    yield return set;
                    yield return set.Push(temp);
                }
            }
            else
            {
                yield return ImmutableStack<TSource>.Empty;
            }
        }
    }
}
