using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns every n'th (intervall) element from the source sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="intervall">the intervall between elements in the source sequences.</param>
        /// <returns>Returns a sequence where only every n'th element (intervall) of source sequnce is used. </returns>
        [Pure]
        public static IEnumerable<TSource> TakeEvery<TSource>(this IEnumerable<TSource> source, int intervall)
        {
            ValidateIntervall(intervall);

            return source.Where((_, index) => index % intervall == 0);
        }

        private static void ValidateIntervall(int intervall)
        {
            if (intervall <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intervall), intervall, "Intervall must be bigger than 0");
            }
        }
    }
}
