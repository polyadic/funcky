namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
        /// <param name="source">An async enumerable.</param>
        /// <param name="action">A synchronous action.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public static IAsyncEnumerable<TSource> Inspect<TSource>(this IAsyncEnumerable<TSource> source, Action<TSource> action)
            => AsyncEnumerable.Create(cancellationToken => source.InspectInternal(action, cancellationToken));

        /// <summary>
        /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
        /// <param name="source">An async enumerable.</param>
        /// <param name="action">An asynchronous action.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public static IAsyncEnumerable<TSource> InspectAwait<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask> action)
            => AsyncEnumerable.Create(cancellationToken => source.InspectAwaitInternal(action, cancellationToken));

        private static async IAsyncEnumerator<TSource> InspectInternal<TSource>(this IAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken)
        {
            await foreach (var item in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                action(item);
                yield return item;
            }
        }

        private static async IAsyncEnumerator<TSource> InspectAwaitInternal<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask> action, CancellationToken cancellationToken)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                await action(item).ConfigureAwait(false);
                yield return item;
            }
        }
    }
}
