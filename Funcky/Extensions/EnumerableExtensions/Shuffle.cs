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
                .ShuffleInternal(new Random());

        private static IEnumerable<T> ShuffleInternal<T>(this IEnumerable<T> source, Random random)
        {
            var list = source.ToList();
            foreach (var (currentIndex, randomIndex) in list.RandomIndex(random))
            {
                yield return list[randomIndex];

                list[randomIndex] = list[currentIndex];
            }
        }

        private static IEnumerable<(int Index, int Random)> RandomIndex<T>(this IReadOnlyCollection<T> buffer, Random random)
            => Enumerable.Range(0, buffer.Count)
                .Select((_, i) => (i, random.Next(i, buffer.Count)));
    }
}
