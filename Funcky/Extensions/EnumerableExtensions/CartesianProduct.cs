using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// CartesianProduct returns a set of ordered pairs where each element of the first sequence is paired with each element of the second sequence.
        /// </summary>
        /// <param name="firstSequence">The first sequence.</param>
        /// <param name="secondSequence">The second sequence.</param>
        /// <typeparam name="TFirstSource">The type of the elements in the first sequence.</typeparam>
        /// <typeparam name="TSecondSource">The type of the elements in the second sequence.</typeparam>
        /// <returns>Returns a list of all combinations as pairs.</returns>
        [Pure]
        public static IEnumerable<(TFirstSource First, TSecondSource Second)> CartesianProduct<TFirstSource, TSecondSource>(
            this IEnumerable<TFirstSource> firstSequence,
            IEnumerable<TSecondSource> secondSequence)
                => CartesianProduct(firstSequence, secondSequence, ValueTuple.Create);

        /// <summary>
        /// CartesianProduct returns a set the projection of ordered pairs where each element of the first sequence is paired with each element of the second sequence.
        /// </summary>
        /// <param name="firstSequence">The first sequence.</param>
        /// <param name="secondSequence">The second sequence.</param>
        /// <param name="resultSelector">A projection which has transforms the input pairs into a result.</param>
        /// <typeparam name="TFirstSource">The type of the elements in the first sequence.</typeparam>
        /// <typeparam name="TSecondSource">The type of the elements in the second sequence.</typeparam>
        /// <typeparam name="TResult">The result type of the given projection.</typeparam>
        /// <returns>sequence of elements returned by <paramref name="resultSelector"/>.</returns>
        [Pure]
        public static IEnumerable<TResult> CartesianProduct<TFirstSource, TSecondSource, TResult>(
            this IEnumerable<TFirstSource> firstSequence,
            IEnumerable<TSecondSource> secondSequence,
            Func<TFirstSource, TSecondSource, TResult> resultSelector)
                => firstSequence.SelectMany(_ => secondSequence, resultSelector);
    }
}
