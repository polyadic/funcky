#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">An async enumerable.</param>
    /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <param name="inspector">A synchronous action.</param>
        /// <returns>Returns an <see cref="IAsyncEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public IAsyncEnumerable<TSource> Inspect(Action<TSource> inspector)
            => InspectInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(inspector));

        /// <summary>
        /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <param name="inspector">An asynchronous action.</param>
        /// <returns>Returns an <see cref="IAsyncEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public IAsyncEnumerable<TSource> InspectAwait(Func<TSource, ValueTask> inspector)
            => InspectAwaitInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(inspector));

        /// <summary>
        /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed whenever enumerating of the result happens.
        /// </summary>
        /// <param name="inspector">An asynchronous action.</param>
        /// <returns>Returns an <see cref="IAsyncEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public IAsyncEnumerable<TSource> InspectAwaitWithCancellation(Func<TSource, CancellationToken, ValueTask> inspector)
            => InspectAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(inspector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async IAsyncEnumerable<TSource> InspectInternal(
            Action<TSource> action,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                action(item);
                yield return item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async IAsyncEnumerable<TSource> InspectAwaitInternal(
            Func<TSource, ValueTask> action,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                await action(item).ConfigureAwait(false);
                yield return item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async IAsyncEnumerable<TSource> InspectAwaitWithCancellationInternal(
            Func<TSource, CancellationToken, ValueTask> action,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                await action(item, cancellationToken).ConfigureAwait(false);
                yield return item;
            }
        }
    }
}
#endif
