using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the given sequence in random Order in O(n).
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
        [Pure]
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source
                .ToList()
                .ShuffleInternal(new Random());

        private static IEnumerable<T> ShuffleInternal<T>(this List<T> source, Random random)
        {
            foreach (var (currentIndex, randomIndex) in source.RandomIndex(random))
            {
                yield return source[randomIndex];

                source[randomIndex] = source[currentIndex];
            }
        }

        private static IEnumerable<(int Index, int Random)> RandomIndex<T>(this IReadOnlyCollection<T> buffer, Random random)
            => Enumerable.Range(0, buffer.Count)
                .Select((_, i) => (i, random.Next(i, buffer.Count)));
    }
}
