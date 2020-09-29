using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// On a rectangular matrix (sequence of sequence where every inner sequence is of the same length) this extension function produces the transposed matrix (rows and columns switched).
        /// PRECONDITION: rectangular matrix -> The result for ragged sequences is not defined, and can change as an implementation detail.
        /// </summary>
        /// <remarks>The transpose extension function only returns a transposed matrix for rectangular matrices. The sequence elements are yielded lazily however the outer sequence will be iterated greedily once to count its length.</remarks>
        /// <param name="source">A source matrix.</param>
        /// <typeparam name="TSource">The type of the elements of the source matrix.</typeparam>
        /// <returns>A partially lazy transposition of a matrix.</returns>
        [Pure]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "We need to know the length of the outer IEnumerable to Chunk correctly, we only iterate the outer sequence, which should be cheap")]
        public static IEnumerable<IEnumerable<TSource>> Transpose<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        {
            return source.Any()
                ? source.Interleave().Chunk(source.Count())
                : Enumerable.Empty<IEnumerable<TSource>>();
        }
    }
}
