using Funcky.Internal.Aggregators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">A sequence of <see cref="int"/> values to determine the average value of.</param>
    extension(IEnumerable<int> source)
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="int"/> values to determine the average value of.</param>
    extension(IEnumerable<Option<int>> source)
    {
        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of <see cref="long"/> values to determine the average value of.</param>
    extension(IEnumerable<long> source)
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="long"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="long"/> values to determine the average value of.</param>
    extension(IEnumerable<Option<long>> source)
    {
        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of <see cref="float"/> values to determine the average value of.</param>
    extension(IEnumerable<float> source)
    {
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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<float> AverageOrNone()
            => source.Aggregate(AverageAggregators.Single, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="float"/> values to determine the average value of.</param>
    extension(IEnumerable<Option<float>> source)
    {
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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<float> AverageOrNone()
            => source.Aggregate(AverageAggregators.Single, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of <see cref="double"/> values to determine the average value of.</param>
    extension(IEnumerable<double> source)
    {
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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Double, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="double"/> values to determine the average value of.</param>
    extension(IEnumerable<Option<double>> source)
    {
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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<double> AverageOrNone()
            => source.Aggregate(AverageAggregators.Double, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the average value of.</param>
    extension(IEnumerable<decimal> source)
    {
        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values.  If the sequence is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<decimal> AverageOrNone()
            => source.Aggregate(AverageAggregators.Decimal, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the average value of.</param>
    extension(IEnumerable<Option<decimal>> source)
    {
        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public Option<decimal> AverageOrNone()
            => source.Aggregate(AverageAggregators.Decimal, (calculator, element) => calculator.Add(element)).Average;
    }

    /// <param name="source">A sequence of values that are used to calculate an average.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, int> selector)
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, Option<int>> selector)
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, long> selector)
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, Option<long>> selector)
            => source.Aggregate(AverageAggregators.Int64, (calculator, element) => calculator.Add(selector(element))).Average;

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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<float> AverageOrNone(Func<TSource, float> selector)
            => source.Aggregate(AverageAggregators.Single, (calculator, element) => calculator.Add(selector(element))).Average;

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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<float> AverageOrNone(Func<TSource, Option<float>> selector)
            => source.Aggregate(AverageAggregators.Single, (calculator, element) => calculator.Add(selector(element))).Average;

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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, double> selector)
            => source.Aggregate(AverageAggregators.Double, (calculator, element) => calculator.Add(selector(element))).Average;

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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<double> AverageOrNone(Func<TSource, Option<double>> selector)
            => source.Aggregate(AverageAggregators.Double, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<decimal> AverageOrNone(Func<TSource, decimal> selector)
            => source.Aggregate(AverageAggregators.Decimal, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence of <typeparamref name ="TSource"/>.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public Option<decimal> AverageOrNone(Func<TSource, Option<decimal>> selector)
            => source.Aggregate(AverageAggregators.Decimal, (calculator, element) => calculator.Add(selector(element))).Average;
    }
}
