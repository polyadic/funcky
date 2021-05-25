using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the async enumerable.</typeparam>
        /// <param name="elements">An async enumerable.</param>
        /// <param name="action">A synchronous action.</param>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public static IAsyncEnumerable<TSource> Inspect<TSource>(this IAsyncEnumerable<TSource> elements, Action<TSource> action)
            => AsyncEnumerable.Create(cancellationToken => elements.InspectInternal(action, cancellationToken));

        /// <summary>
        /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the async enumerable.</typeparam>
        /// <param name="elements">An async enumerable.</param>
        /// <param name="action">An asynchronous action.</param>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public static IAsyncEnumerable<TSource> InspectAwait<TSource>(this IAsyncEnumerable<TSource> elements, Func<TSource, ValueTask> action)
            => AsyncEnumerable.Create(cancellationToken => elements.InspectAwaitInternal(action, cancellationToken));

        private static async IAsyncEnumerator<TSource> InspectInternal<TSource>(this IAsyncEnumerable<TSource> elements, Action<TSource> action, CancellationToken cancellationToken)
        {
            await foreach (var element in elements.WithCancellation(cancellationToken))
            {
                action(element);
                yield return element;
            }
        }

        private static async IAsyncEnumerator<TSource> InspectAwaitInternal<TSource>(this IAsyncEnumerable<TSource> elements, Func<TSource, ValueTask> action, CancellationToken cancellationToken)
        {
            await foreach (var element in elements.WithCancellation(cancellationToken))
            {
                await action(element);
                yield return element;
            }
        }
    }
}
