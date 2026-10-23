#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="left">The left sequence to merge.</param>
    /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
    extension<TLeft>(IAsyncEnumerable<TLeft> left)
        where TLeft : notnull
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public IAsyncEnumerable<EitherOrBoth<TLeft, TRight>> ZipLongest<TRight>(IAsyncEnumerable<TRight> right)
            where TRight : notnull
            => left.ZipLongest(right, Identity);

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public IAsyncEnumerable<TResult> ZipLongest<TRight, TResult>(IAsyncEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
            where TRight : notnull
            => ZipLongestInternal(ArgumentValidator.NotNull(left), ArgumentValidator.NotNull(right), ArgumentValidator.NotNull(resultSelector));
    }

    private static async IAsyncEnumerable<TResult> ZipLongestInternal<TLeft, TRight, TResult>(IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TRight : notnull
    {
        #pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var leftEnumerator = left.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
        await using var rightEnumerator = right.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
        #pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        while ((await MoveNextOrNone(leftEnumerator, rightEnumerator).ConfigureAwait(false)).TryGetValue(out var next))
        {
            yield return resultSelector(next);
        }
    }

    private static async ValueTask<Option<EitherOrBoth<TLeft, TRight>>> MoveNextOrNone<TLeft, TRight>(ConfiguredCancelableAsyncEnumerable<TLeft>.Enumerator leftEnumerator, ConfiguredCancelableAsyncEnumerable<TRight>.Enumerator rightEnumerator)
        where TLeft : notnull
        where TRight : notnull
        => EitherOrBoth.FromOptions(await ReadNext(leftEnumerator).ConfigureAwait(false), await ReadNext(rightEnumerator).ConfigureAwait(false));

    private static async ValueTask<Option<TSource>> ReadNext<TSource>(ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator enumerator)
        where TSource : notnull
        => await enumerator.MoveNextAsync()
            ? enumerator.Current
            : Option<TSource>.None;
}
#endif
