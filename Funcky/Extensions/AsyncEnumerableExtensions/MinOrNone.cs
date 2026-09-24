#if INTEGRATED_ASYNC
using Funcky.Internal.Aggregators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the minimum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TSource>> MinOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => source.MinOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Returns the minimum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TSource>> MinOrNoneAsync<TSource>(this IAsyncEnumerable<Option<TSource>> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => source.WhereSelect().MinOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.Select(selector).AggregateAsync(Option<TResult>.None, MinAggregator.Aggregate, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAwaitAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.Select(selector).AggregateAsync(Option<TResult>.None, async (min, current, _) => MinAggregator.Aggregate(min, await current.ConfigureAwait(false)), cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAwaitWithCancellationAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.Select(selector).AggregateAsync(Option<TResult>.None, MinAggregator.Aggregate, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelect(selector).MinOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAwaitAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelectAwait(selector).MinOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The minimum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MinOrNoneAwaitWithCancellationAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelectAwaitWithCancellation(selector).MinOrNoneAsync(Identity, cancellationToken);
}
#endif
