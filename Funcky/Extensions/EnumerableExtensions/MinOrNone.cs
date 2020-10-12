using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.GenericConstraints;
using Funcky.Monads;
using static System.Math ;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the minimum value in a sequence of Int32 values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Int32 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone(this IEnumerable<int> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional Int32 values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Int32 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int32 value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(Option<int>.None(), (min, current) => Option.Some(min.Match(selector(current), m => Min(selector(current), m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int32 value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of Int64 values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Int64 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone(this IEnumerable<long> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional Int64 values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Int64 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int64 value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(Option<long>.None(), (min, current) => Option.Some(min.Match(selector(current), m => Min(selector(current), m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int64 value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of Double values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Double values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone(this IEnumerable<double> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional Double values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Double values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Double value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(Option<double>.None(), (min, current) => Option.Some(min.Match(selector(current), m => DefaultComparerMin(selector(current), m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Double value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of Single values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Single values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone(this IEnumerable<float> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional Single values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Single values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Single value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(Option<float>.None(), (min, current) => Option.Some(min.Match(selector(current), m => DefaultComparerMin(selector(current), m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Single value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of Decimal values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Decimal values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone(this IEnumerable<decimal> source)
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional Decimal values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Decimal values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Decimal value. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(Option<decimal>.None(), (min, current) => Option.Some(min.Match(selector(current), m => Min(selector(current), m))));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Decimal value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of generic values compared by a Comparer{T}. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source, RequireClass<TSource>? ω = null)
            where TSource : class
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of generic values compared by a Comparer{T}. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional generic values compared by a Comparer{T}. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of optional generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireClass<TSource>? ω = null)
            where TSource : class
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional generic values compared by a Comparer{T}. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
            => source.WhereSelect(Identity).MinOrNone(Identity);

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireClass<TResult>? ω = null)
            where TResult : class
            => source.Aggregate(Option<TResult>.None(), (min, current) => Option.Some(min.Match(selector(current), m => Comparer<TResult>.Default.Compare(m, selector(current)) < 0 ? selector(current) : m)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
            => source.Aggregate(Option<TResult>.None(), (min, current) => Option.Some(min.Match(selector(current), m => Comparer<TResult>.Default.Compare(m, selector(current)) < 0 ? selector(current) : m)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireClass<TResult>? ω = null)
            where TResult : class
            => source.WhereSelect(selector).MinOrNone(Identity);

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
            => source.WhereSelect(selector).MinOrNone(Identity);

        private static float DefaultComparerMin(float left, float right)
            => Comparer<float>.Default.Compare(left, right) < 0 ? left : right;

        private static double DefaultComparerMin(double left, double right)
            => Comparer<double>.Default.Compare(left, right) < 0 ? left : right;
    }
}
