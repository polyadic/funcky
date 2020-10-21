using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;
using static Funcky.Monads.Option;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<int> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="int"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<int>> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<long> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="long"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<long>> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<float> source)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(element)).Average;

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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<Option<float>> source)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(element)).Average;

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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<double> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

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
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<double>> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values.  If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="long"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<decimal> source)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="long"/> values to determine the average value of.</param>
        /// <returns>The average value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<Option<decimal>> source)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(element)).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="int"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="long"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="float"/> values that are obtained by invoking a transform function on each element of the input sequence.
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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="float"/> values that are obtained by invoking a transform function on each element of the input sequence.
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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="double"/> values that are obtained by invoking a transform function on each element of the input sequence.
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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="double"/> values that are obtained by invoking a transform function on each element of the input sequence.
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
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        /// <summary>
        /// Computes the average of a sequence of optional <see cref="decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values that are used to calculate an average.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values or None.</returns>
        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        private class AverageCalculatorDouble
        {
            public static readonly AverageCalculatorDouble Empty = new AverageCalculatorDouble();

            private readonly int _count;
            private readonly Option<double> _sum;

            public AverageCalculatorDouble(int count = default, Option<double> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<double> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorDouble Add(int term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<int> term)
                => term.Match(this, Add);

            public AverageCalculatorDouble Add(long term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<long> term)
                => term.Match(this, Add);

            public AverageCalculatorDouble Add(double term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<double> term)
                => term.Match(this, Add);
        }

        private class AverageCalculatorFloat
        {
            public static readonly AverageCalculatorFloat Empty = new AverageCalculatorFloat();

            private readonly Option<float> _sum;
            private readonly int _count;

            public AverageCalculatorFloat(int count = default, Option<float> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<float> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorFloat Add(float term)
                => new AverageCalculatorFloat(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorFloat Add(Option<float> term)
                => term.Match(this, Add);
        }

        private class AverageCalculatorDecimal
        {
            public static readonly AverageCalculatorDecimal Empty = new AverageCalculatorDecimal();

            private readonly Option<decimal> _sum;
            private readonly int _count;

            public AverageCalculatorDecimal(int count = default, Option<decimal> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<decimal> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorDecimal Add(decimal term)
                => new AverageCalculatorDecimal(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDecimal Add(Option<decimal> term)
                => term.Match(this, Add);
        }
    }
}
