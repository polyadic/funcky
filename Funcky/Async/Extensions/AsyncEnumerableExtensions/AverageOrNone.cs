using Funcky.Internal.Aggregators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Computes the average of a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of <see cref="int"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<int> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="int"/> values. If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional <see cref="int"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<Option<int>> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="long"/> values. If the sequence is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of <see cref="long"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<long> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional <see cref="long"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<Option<long>> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="float"/> values. If the sequence is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <param name="source">A sequence of <see cref="float"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<float>> AverageOrNoneAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="float"/> values. If the sequence only consists of none or is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <param name="source">A sequence of optional <see cref="float"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<float>> AverageOrNoneAsync(this IAsyncEnumerable<Option<float>> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of <see cref="double"/> values. If the sequence is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <param name="source">A sequence of <see cref="double"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="double"/> values. If the sequence only consists of none or is empty it returns None.
    /// The rules for floating point arithmetic apply see remarks for detail.
    /// </summary>
    /// <remarks>
    /// Any sequence containing at least one NaN value will evaluate to NaN.
    /// A sequence containing at least one +Infinity will always evaluate to an average of +infinity.
    /// A sequence containing at least one -Infinity will always evaluate to an average of -infinity.
    /// A sequence containing at least one of each +Infinity and -Infinity values will always evaluate to NaN.
    /// </remarks>
    /// <param name="source">A sequence of optional <see cref="double"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<double>> AverageOrNoneAsync(this IAsyncEnumerable<Option<double>> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="decimal"/> values.  If the sequence is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<decimal>> AverageOrNoneAsync(this IAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

    /// <summary>
    /// Computes the average of a sequence of optional <see cref="decimal"/> values. If the sequence only consists of none or is empty it returns None.
    /// </summary>
    /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the average value of.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The average value in the sequence or None.</returns>
    [Pure]
    public static async ValueTask<Option<decimal>> AverageOrNoneAsync(this IAsyncEnumerable<Option<decimal>> source, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, (calculator, element) => calculator.Add(element), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<int>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<long>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<float>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<float>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<float>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(FloatAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<double>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<double>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DoubleAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<decimal>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;

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
    public static async ValueTask<Option<decimal>> AverageOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<decimal>> selector, CancellationToken cancellationToken = default)
        => (await source.AggregateAsync(DecimalAverageAggregator.Empty, (calculator, element) => calculator.Add(selector(element)), cancellationToken).ConfigureAwait(false)).Average;
}
