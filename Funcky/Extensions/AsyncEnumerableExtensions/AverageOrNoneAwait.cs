#if INTEGRATED_ASYNC
using Funcky.Internal.Aggregators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Computes the average of a sequence of <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<int>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<int>>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<long>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<long>>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="float"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<float>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<float>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="float"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence only consists of none or is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<float>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<float>>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="double"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<double>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="double"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence only consists of none or is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<double>>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<decimal>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<decimal>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
    /// If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average of the sequence of values or None.</returns>
    [Pure]
    public static async ValueTask<Option<decimal>> AverageOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<decimal>>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, async (calculator, element, _) => calculator.Add(await selector(element).ConfigureAwait(false)), cancellationToken).ConfigureAwait(false)).Average;
}
#endif
