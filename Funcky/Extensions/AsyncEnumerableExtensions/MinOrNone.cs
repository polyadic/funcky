#if ASYNC_SUPPORTED
using Funcky.Internal.Aggregators;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the minimum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TSource>> MinOrNoneAsync(CancellationToken cancellationToken = default)
            => source.MinOrNoneAsync(Identity, cancellationToken);
    }

    /// <param name="source">A sequence of optional generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    extension<TSource>(IAsyncEnumerable<Option<TSource>> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the minimum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TSource>> MinOrNoneAsync(CancellationToken cancellationToken = default)
            => source.WhereSelect().MinOrNoneAsync(Identity, cancellationToken);
    }

    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TResult>> MinOrNoneAsync<TResult>(Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.Select(selector).AggregateAsync(Option<TResult>.None, MinAggregator.Aggregate, cancellationToken);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        [Obsolete("Use MinOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TResult>> MinOrNoneAwaitAsync<TResult>(Func<TSource, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.MinOrNoneAsync((item, _) => selector(item), cancellationToken);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TResult>> MinOrNoneAsync<TResult>(Func<TSource, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.Select(selector).AggregateAsync(Option<TResult>.None, MinAggregator.Aggregate, cancellationToken);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TResult>> MinOrNoneAsync<TResult>(Func<TSource, Option<TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.WhereSelect(selector).MinOrNoneAsync(Identity, cancellationToken);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        [Obsolete("Use MinOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TResult>> MinOrNoneAwaitAsync<TResult>(Func<TSource, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => WhereSelectAwaitInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector), cancellationToken).MinOrNoneAsync(Identity, cancellationToken);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public ValueTask<Option<TResult>> MinOrNoneAsync<TResult>(Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.WhereSelect(selector).MinOrNoneAsync(Identity, cancellationToken);

        /// <inheritdoc cref="MinOrNoneAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,CancellationToken,ValueTask{TResult}},CancellationToken)"/>
        [Pure]
        [Obsolete("Use MinOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TResult>> MinOrNoneAwaitWithCancellationAsync<TResult>(Func<TSource, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.MinOrNoneAsync(selector, cancellationToken);

        /// <inheritdoc cref="MinOrNoneAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,CancellationToken,ValueTask{Option{TResult}}},CancellationToken)"/>
        [Pure]
        [Obsolete("Use MinOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TResult>> MinOrNoneAwaitWithCancellationAsync<TResult>(Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
            where TResult : notnull
            => source.MinOrNoneAsync(selector, cancellationToken);
    }
}
#endif
