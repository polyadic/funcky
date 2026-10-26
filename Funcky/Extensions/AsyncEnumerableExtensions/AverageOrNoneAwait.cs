#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<int>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<Option<int>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<long>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<Option<long>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<float>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<float>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<float>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<Option<float>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<double>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<Option<double>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<decimal>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<decimal>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<decimal>> AverageOrNoneAwaitAsync(Func<TSource, ValueTask<Option<decimal>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync((element, _) => selector(element), cancellationToken);
    }

    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<int>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<Option<int>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<long>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<Option<long>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<float>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<float>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<float>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<Option<float>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<double>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

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
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<double>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<Option<double>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<decimal>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<decimal>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        [Obsolete("Use AverageOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<decimal>> AverageOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<Option<decimal>>> selector, CancellationToken cancellationToken = default)
            => source.AverageOrNoneAsync(selector, cancellationToken);
    }
}
#endif
