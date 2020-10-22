using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone(this IEnumerable<int> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional <see cref="int"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="int"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="int"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Select(selector).Aggregate(Option<int>.None(), (min, current) => Option.Some(min.Match(current, m => Min(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="int"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="long"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone(this IEnumerable<long> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="long"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="long"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Select(selector).Aggregate(Option<long>.None(), (min, current) => Option.Some(min.Match(current, m => Min(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="long"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="double"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone(this IEnumerable<double> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional <see cref="double"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="double"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="double"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Select(selector).Aggregate(Option<double>.None(), (min, current) => Option.Some(min.Match(current, m => Min(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="double"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="float"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="float"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone(this IEnumerable<float> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional <see cref="float"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="float"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="float"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Select(selector).Aggregate(Option<float>.None(), (min, current) => Option.Some(min.Match(current, m => Min(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="float"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="decimal"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone(this IEnumerable<decimal> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional <see cref="decimal"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="decimal"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Select(selector).Aggregate(Option<decimal>.None(), (min, current) => Option.Some(min.Match(current, m => Min(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional <see cref="decimal"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source)
            where TSource : notnull
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : notnull
            => source.Select(selector).Aggregate(Option<TResult>.None(), (min, current) => Option.Some(min.Match(current, m => Min(m, current))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => source.WhereSelect(selector).MinOrNone(Identity);

        // For floats this defines a total order where NaN comes before negative infinity
        private static TSource Min<TSource>(TSource left, TSource right)
            => Comparer<TSource>.Default.Compare(left, right) < 0 ? left : right;
    }
}
