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
        /// Returns the maximum value in a sequence of <see cref="int"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone(this IEnumerable<int> source)
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional <see cref="int"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="int"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="int"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Select(selector).Aggregate(Option<int>.None(), (max, current) => Option.Some(max.Match(current, m => Max(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="int"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.WhereSelect(selector).MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="long"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone(this IEnumerable<long> source)
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional <see cref="long"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="long"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="long"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Select(selector).Aggregate(Option<long>.None(), (max, current) => Option.Some(max.Match(current, m => Max(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="long"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the maximum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.WhereSelect(selector).MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="double"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone(this IEnumerable<double> source)
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional <see cref="double"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="double"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="double"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Select(selector).Aggregate(Option<double>.None(), (max, current) => Option.Some(max.Match(current, m => Max(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="double"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.WhereSelect(selector).MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="float"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="float"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone(this IEnumerable<float> source)
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional <see cref="float"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="float"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="float"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Select(selector).Aggregate(Option<float>.None(), (max, current) => Option.Some(max.Match(current, m => Max(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="float"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.WhereSelect(selector).MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="decimal"/> values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone(this IEnumerable<decimal> source)
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional <see cref="decimal"/> values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional <see cref="decimal"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="decimal"/> value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Select(selector).Aggregate(Option<decimal>.None(), (max, current) => Option.Some(max.Match(current, m => Max(current, m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum optional <see cref="decimal"/> value. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.WhereSelect(selector).MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional generic values of TSource to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source)
            where TSource : notnull
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : notnull
            => source.Select(selector).Aggregate(Option<TResult>.None(), (max, current) => Option.Some(max.Match(current, m => Max(m, current))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => source.WhereSelect(selector).MaxOrNone(Identity);

        private static TSource Max<TSource>(TSource left, TSource right)
            => Comparer<TSource>.Default.Compare(left, right) > 0 ? left : right;
    }
}
