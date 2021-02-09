using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="source">The source sequence to merge.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="other">The other sequence to merge.</param>
        /// <typeparam name="TOther">Type of the elements in <paramref name="other"/> sequence.</typeparam>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static IEnumerable<(Option<TSource> First, Option<TOther> Second)> ZipLongest<TSource, TOther>(this IEnumerable<TSource> source, IEnumerable<TOther> other)
            where TSource : notnull
            where TOther : notnull
            => source.ZipLongest(other, ValueTuple.Create);

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="source">The source sequence to merge.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="other">The other sequence to merge.</param>
        /// <typeparam name="TOther">Type of the elements in <paramref name="other"/> sequence.</typeparam>
        /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static IEnumerable<TResult> ZipLongest<TSource, TOther, TResult>(this IEnumerable<TSource> source, IEnumerable<TOther> other, Func<Option<TSource>, Option<TOther>, TResult> resultSelector)
            where TSource : notnull
            where TOther : notnull
        {
            using var sourceEnumerator = source.GetEnumerator();
            using var otherEnumerator = other.GetEnumerator();

            while (true)
            {
                var sourceElement = ReadNext(sourceEnumerator);
                var otherElement = ReadNext(otherEnumerator);

                if (sourceElement.Match(False, True) || otherElement.Match(False, True))
                {
                    yield return resultSelector(sourceElement, otherElement);
                }
                else
                {
                    yield break;
                }
            }
        }

        private static Option<TSource> ReadNext<TSource>(IEnumerator<TSource> enumerator)
            where TSource : notnull
            => enumerator.MoveNext()
                ? enumerator.Current
                : Option<TSource>.None();
    }
}
