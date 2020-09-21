using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns every n'th (interval) element from the source sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="interval">the interval between elements in the source sequences.</param>
        /// <returns>Returns a sequence where only every n'th element (interval) of source sequnce is used. </returns>
        [Pure]
        public static IEnumerable<TSource> TakeEvery<TSource>(this IEnumerable<TSource> source, int interval)
        {
            ValidateInterval(interval);

            return source.Where((_, index) => index % interval == 0);
        }

        private static void ValidateInterval(int interval)
        {
            if (interval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval must be bigger than 0");
            }
        }
    }
}
