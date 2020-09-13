using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;
using static Funcky.Extensions.PredicateExtensions;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken<TSource>(True), cancellationToken);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (await predicate(item, cancellationToken))
                {
                    return Option.Some(item);
                }
            }

            return Option<TSource>.None();
        }
    }
}
