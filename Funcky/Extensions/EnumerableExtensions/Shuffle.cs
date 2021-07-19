using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the given sequence in random Order in O(n).
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
        /// <remarks>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method directly or by using foreach.</remarks>
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
