#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
    /// The side effect will be executed whenever enumerating of the result happens.
    /// </summary>
    /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
    /// <param name="source">An async enumerable.</param>
    /// <param name="inspector">A synchronous action.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Inspect<TSource>(this IAsyncEnumerable<TSource> source, Action<TSource> inspector)
        => InspectInternal(source, inspector);

    /// <summary>
    /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
    /// The side effect will be executed whenever enumerating of the result happens.
    /// </summary>
    /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
    /// <param name="source">An async enumerable.</param>
    /// <param name="inspector">An asynchronous action.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> InspectAwait<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask> inspector)
        => InspectAwaitInternal(source, inspector);

    /// <summary>
    /// An IEnumerable that calls and awaits the function on each element before yielding it. It can be used to encode side effects without enumerating.
    /// The side effect will be executed whenever enumerating of the result happens.
    /// </summary>
    /// <typeparam name="TSource">The inner type of the async enumerable.</typeparam>
    /// <param name="source">An async enumerable.</param>
    /// <param name="inspector">An asynchronous action.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> InspectAwaitWithCancellation<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask> inspector)
        => InspectAwaitWithCancellationInternal(source, inspector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async IAsyncEnumerable<TSource> InspectInternal<TSource>(
        this IAsyncEnumerable<TSource> source,
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
    private static async IAsyncEnumerable<TSource> InspectAwaitInternal<TSource>(
        this IAsyncEnumerable<TSource> source,
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
    private static async IAsyncEnumerable<TSource> InspectAwaitWithCancellationInternal<TSource>(
        this IAsyncEnumerable<TSource> source,
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
#endif
