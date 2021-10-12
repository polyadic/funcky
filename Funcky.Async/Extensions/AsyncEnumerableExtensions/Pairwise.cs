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
        private static async IAsyncEnumerator<TResult> PairwiseInternal<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector, CancellationToken cancellationToken)
        {
            #pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var enumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
            #pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            if (await enumerator.MoveNextAsync() == false)
            {
                yield break;
            }

            for (var previous = enumerator.Current; await enumerator.MoveNextAsync(); previous = enumerator.Current)
            {
                yield return resultSelector(previous, enumerator.Current);
            }
        }
    }
}
