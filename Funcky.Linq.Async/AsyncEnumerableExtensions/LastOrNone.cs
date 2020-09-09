using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;

namespace Funcky.Linq.Async
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.Select(Option.Some).LastOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Returns the last element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.Where(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.WhereAwait(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> LastOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.WhereAwaitWithCancellation(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken);
    }
}
