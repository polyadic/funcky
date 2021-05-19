using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Funcky.Internal;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence of pair-wise tuples from the underlying sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        [Pure]
        public static IAsyncEnumerable<(TSource Front, TSource Back)> Pairwise<TSource>(this IAsyncEnumerable<TSource> source)
            => Pairwise(source, ValueTuple.Create);

        /// <summary>
        /// Applies a function to the element and its successor pairwise from the underlying sequence and returns a new sequence with these results.
        /// I.e. the resulting sequence has one element less then source sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="resultSelector">A function of type (TSource, TSource) -> TResult which creates the results from the pairs.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the result Sequence.</typeparam>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        public static IAsyncEnumerable<TResult> Pairwise<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
            => AsyncEnumerable.Create(cancellationToken => PairwiseInternal(source, resultSelector, cancellationToken));

        [Pure]
#pragma warning disable 8425
        public static async IAsyncEnumerator<TResult> PairwiseInternal<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector, CancellationToken cancellationToken)
        {
            await using var enumerator = source.GetAsyncEnumerator(cancellationToken);

            if (await enumerator.MoveNextAsync() == false)
            {
                yield break;
            }

            for (var previous = enumerator.Current; await enumerator.MoveNextAsync(); previous = enumerator.Current)
            {
                yield return resultSelector(previous, enumerator.Current);
            }
        }
#pragma warning restore 8425
    }
}
