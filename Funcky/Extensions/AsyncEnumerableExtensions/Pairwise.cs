#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns a sequence of pair-wise tuples from the underlying sequence.
        /// </summary>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        [Pure]
        public IAsyncEnumerable<(TSource Front, TSource Back)> Pairwise()
            => Pairwise(source, ValueTuple.Create);

        /// <summary>
        /// Applies a function to the element and its successor pairwise from the underlying sequence and returns a new sequence with these results.
        /// I.e. the resulting sequence has one element less then source sequence.
        /// </summary>
        /// <param name="resultSelector">A function of type (TSource, TSource) -> TResult which creates the results from the pairs.</param>
        /// <typeparam name="TResult">The type of the elements in the result Sequence.</typeparam>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        [Pure]
        public IAsyncEnumerable<TResult> Pairwise<TResult>(Func<TSource, TSource, TResult> resultSelector)
            => PairwiseInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(resultSelector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async IAsyncEnumerable<TResult> PairwiseInternal<TResult>(
            Func<TSource, TSource, TResult> resultSelector,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
#endif
