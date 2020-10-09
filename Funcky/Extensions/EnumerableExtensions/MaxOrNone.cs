using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.GenericConstraints;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the maximum value in a sequence of Int32 values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Int32 values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone(this IEnumerable<int> source)
            => source.Aggregate(Option<int>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional Int32 values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Int32 values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        /// <summary>
        /// Returns the maximum value in a sequence of Int64 values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Int64 values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone(this IEnumerable<long> source)
            => source.Aggregate(Option<long>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional Int64 values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Int64 values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        /// <summary>
        /// Returns the maximum value in a sequence of Double values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Double values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone(this IEnumerable<double> source)
            => source.Aggregate(Option<double>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional Double values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Double values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        /// <summary>
        /// Returns the maximum value in a sequence of Single values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Single values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone(this IEnumerable<float> source)
            => source.Aggregate(Option<float>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional Single values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Single values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        /// <summary>
        /// Returns the maximum value in a sequence of Decimal values. If the sequence is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of Decimal values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone(this IEnumerable<decimal> source)
            => source.Aggregate(Option<decimal>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional Decimal values. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional Decimal values to determine the minimum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        /// <summary>
        /// Returns the maximum value in a sequence of generic values compared by a Comparer{T}. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source, RequireClass<TSource>? ω = null)
            where TSource : class
            => source.Aggregate(Option<TSource>.None(), (max, current) => Option.Some(max.Match(current, m => Comparer<TSource>.Default.Compare(m, current) > 0 ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of generic values compared by a Comparer{T}. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
            => source.Aggregate(Option<TSource>.None(), (max, current) => Option.Some(max.Match(current, m => Comparer<TSource>.Default.Compare(m, current) > 0 ? current : m)));

        /// <summary>
        /// Returns the maximum value in a sequence of optional generic values compared by a Comparer{T}. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of optional generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireClass<TSource>? ω = null)
            where TSource : class
            => source.Aggregate(Option<TSource>.None(), (max, current) => (from m in max from c in current select Comparer<TSource>.Default.Compare(m, c) > 0 ? c : m).OrElse(current));

        /// <summary>
        /// Returns the maximum value in a sequence of optional generic values compared by a Comparer{T}. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of optional generic values of TSource to determine the minimum value of.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
            => source.Aggregate(Option<TSource>.None(), (max, current) => (from m in max from c in current select Comparer<TSource>.Default.Compare(m, c) > 0 ? c : m).OrElse(current));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int32 value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(Option<int>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int32 value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.Aggregate(Option<int>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int64 value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(Option<long>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Int64 value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.Aggregate(Option<long>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Single value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(Option<float>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Single value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.Aggregate(Option<float>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Double value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(Option<double>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Double value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.Aggregate(Option<double>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Decimal value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(Option<decimal>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum optional Decimal value. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values of TSource to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.Aggregate(Option<decimal>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireClass<TResult>? ω = null)
            where TResult : class
            => source.Aggregate(Option<TResult>.None(), (max, current) => Option.Some(max.Match(selector(current), m => Comparer<TResult>.Default.Compare(m, selector(current)) > 0 ? selector(current) : m)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
            => source.Aggregate(Option<TResult>.None(), (max, current) => Option.Some(max.Match(selector(current), m => Comparer<TResult>.Default.Compare(m, selector(current)) > 0 ? selector(current) : m)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireClass<TResult>? ω = null)
            where TResult : class
            => source.Aggregate(Option<TResult>.None(), (max, current) => (from m in max from c in selector(current) select Comparer<TResult>.Default.Compare(m, c) > 0 ? c : m).OrElse(selector(current)));

        /// <summary>
        ///  Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a Comparer{T}. If the transforemd sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="ω">Dummy Parameter, do not use.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
            => source.Aggregate(Option<TResult>.None(), (max, current) => (from m in max from c in selector(current) select Comparer<TResult>.Default.Compare(m, c) > 0 ? c : m).OrElse(selector(current)));
    }
}
