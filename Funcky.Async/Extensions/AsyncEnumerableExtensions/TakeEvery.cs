using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Internal;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns every n'th (interval) element from the source sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="interval">The interval between elements in the source sequences.</param>
        /// <returns>Returns a sequence where only every n'th element (interval) of source sequence is used. </returns>
        [Pure]
        public static IAsyncEnumerable<TSource> TakeEvery<TSource>(this IAsyncEnumerable<TSource> source, int interval)
        {
            ValidateInterval(interval);

            return AsyncEnumerable.Create(cancellationToken => TakeEveryEnumerable(source, interval, cancellationToken));
        }

        #pragma warning disable 8425
        private static async IAsyncEnumerator<TSource> TakeEveryEnumerable<TSource>(this IAsyncEnumerable<TSource> source, int interval, CancellationToken cancellationToken = default)
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
        #pragma warning restore 8425

        private static void ValidateInterval(int interval)
        {
            if (interval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval must be bigger than 0");
            }
        }
    }
}
