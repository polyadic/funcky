using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funcky.DataTypes;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="left">The left sequence to merge.</param>
        /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static IAsyncEnumerable<EitherOrBoth<TLeft, TRight>> ZipLongest<TLeft, TRight>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right)
            where TLeft : notnull
            where TRight : notnull
            => left.ZipLongest(right, Identity);

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="left">The left sequence to merge.</param>
        /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static async IAsyncEnumerable<TResult> ZipLongest<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
            where TLeft : notnull
            where TRight : notnull
        {
            await using var leftEnumerator = left.GetAsyncEnumerator();
            await using var rightEnumerator = right.GetAsyncEnumerator();

            for (var next = await MoveNextOrNone(leftEnumerator, rightEnumerator); next.Match(false, True); next = await MoveNextOrNone(leftEnumerator, rightEnumerator))
            {
                yield return resultSelector(next.GetOrElse(() => throw new Exception("Cannot happen.")));
            }
        }

        private static async ValueTask<Option<EitherOrBoth<TLeft, TRight>>> MoveNextOrNone<TLeft, TRight>(IAsyncEnumerator<TLeft> leftEnumerator, IAsyncEnumerator<TRight> rightEnumerator)
            where TLeft : notnull
            where TRight : notnull
            => EitherOrBoth.FromOptions(await ReadNext(leftEnumerator), await ReadNext(rightEnumerator));

        private static async ValueTask<Option<TSource>> ReadNext<TSource>(IAsyncEnumerator<TSource> enumerator)
            where TSource : notnull
            => await enumerator.MoveNextAsync()
                ? enumerator.Current
                : Option<TSource>.None;
    }
}
