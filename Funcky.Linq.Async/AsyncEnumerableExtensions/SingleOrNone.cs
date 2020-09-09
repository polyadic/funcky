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
        /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is more than one element in the sequence.</exception>
        [Pure]
        public static ValueTask<Option<TSource>> SingleOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.Select(Option.Some).SingleOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if more than one element satisfies the condition.</exception>
        [Pure]
        public static ValueTask<Option<TSource>> SingleOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.Where(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="SingleOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> SingleOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.WhereAwait(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="SingleOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> SingleOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> inputs, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => inputs.WhereAwaitWithCancellation(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken);
    }
}
