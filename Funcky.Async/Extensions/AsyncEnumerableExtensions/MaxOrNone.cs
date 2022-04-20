#pragma warning disable RS0026

using Funcky.Internal.Aggregators;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the maximum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TSource>> MaxOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => source.MaxOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Returns the maximum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional generic values of TSource to determine the maximum value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TSource>> MaxOrNoneAsync<TSource>(this IAsyncEnumerable<Option<TSource>> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => source.WhereSelect().MaxOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.Select(selector).AggregateAsync(Option<TResult>.None, MaxAggregator.Aggregate, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAwaitAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.SelectAwait(selector).AggregateAsync(Option<TResult>.None, MaxAggregator.Aggregate, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAwaitWithCancellationAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.SelectAwaitWithCancellation(selector).AggregateAsync(Option<TResult>.None, MaxAggregator.Aggregate, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelect(selector).MaxOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAwaitAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelectAwait(selector).MaxOrNoneAsync(Identity, cancellationToken);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The maximum value in the sequence or None.</returns>
    [Pure]
    public static ValueTask<Option<TResult>> MaxOrNoneAwaitWithCancellationAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector, CancellationToken cancellationToken = default)
        where TResult : notnull
        => source.WhereSelectAwaitWithCancellation(selector).MaxOrNoneAsync(Identity, cancellationToken);
}
