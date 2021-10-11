using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
            where TSource : notnull
            => await source.Select(Option.Some).FirstOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => await source.Where(predicate).Select(Option.Some).FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => await source.WhereAwait(predicate).Select(Option.Some).FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => await source.WhereAwaitWithCancellation(predicate).Select(Option.Some).FirstOrDefaultAsync(cancellationToken);
    }
}
