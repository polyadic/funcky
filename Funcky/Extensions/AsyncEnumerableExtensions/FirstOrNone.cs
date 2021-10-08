namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken<TSource>(True), cancellationToken);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static ValueTask<Option<TSource>> FirstOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.FirstOrNoneAwaitWithCancellationAsync(ToAsyncPredicateWithCancellationToken(predicate), cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Threading.CancellationToken)"/>
        [Pure]
        public static async ValueTask<Option<TSource>> FirstOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                if (await predicate(item, cancellationToken).ConfigureAwait(false))
                {
                    return item;
                }
            }

            return Option<TSource>.None();
        }
    }
}
