using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns every n'th (interval) element from the source sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="interval">the interval between elements in the source sequences.</param>
        /// <returns>Returns a sequence where only every n'th element (interval) of source sequnce is used. </returns>
        [Pure]
        public static async IAsyncEnumerable<TSource> TakeEvery<TSource>(this IAsyncEnumerable<TSource> source, int interval, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            var currentIndex = 0;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (currentIndex % interval == 0)
                {
                    yield return item;
                }

                currentIndex++;
            }
        }
    }
}
