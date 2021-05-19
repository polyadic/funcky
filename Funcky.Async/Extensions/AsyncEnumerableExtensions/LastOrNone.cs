using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.LastOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken<TSource>(True), cancellationToken);

        /// <summary>
        /// Returns the last element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.LastOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.LastOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static async ValueTask<Option<TSource>> LastOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            var lastItem = Option<TSource>.None();

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (await predicate(item, cancellationToken))
                {
                    lastItem = item;
                }
            }

            return lastItem;
        }
    }
}
