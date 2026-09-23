#if INTEGRATED_ASYNC
using Funcky.RetryPolicies;

namespace Funcky;

public static partial class AsyncFunctional
{
    /// <summary>
    /// Calls the given <paramref name="producer"/> over and over until it returns a value or the <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <remarks>
    /// This overload never gives up and never waits between attempts. Use the overload with an <see cref="IRetryPolicy"/>
    /// when the producer can fail permanently or should not be called in a tight loop.
    /// </remarks>
    public static async ValueTask<TResult> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Sequence
            .Cycle(producer)
            .ToAsyncEnumerable()
            .Select(ProduceUnlessCancelled)
            .WhereSelect(Identity)
            .FirstAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public static async ValueTask<Option<TResult>> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await AsyncSequence
            .Return(await producer().ConfigureAwait(false))
            .Concat(TailRetriesAsync(producer, retryPolicy, cancellationToken))
            .WhereSelect(Identity)
            .FirstOrNoneAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static ValueTask<Option<TResult>> ProduceUnlessCancelled<TResult>(Func<ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken)
        where TResult : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        return producer();
    }

    private static IAsyncEnumerable<Option<TResult>> TailRetriesAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        where TResult : notnull
        => Retries(retryPolicy)
            .Select((int item, CancellationToken _) => ProduceDelayedAsync(producer, retryPolicy, cancellationToken)(item));

    private static IAsyncEnumerable<int> Retries(IRetryPolicy retryPolicy)
        => AsyncEnumerable.Range(0, retryPolicy.MaxRetries);

    private static Func<int, ValueTask<Option<TResult>>> ProduceDelayedAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        where TResult : notnull
        => async retryCount =>
        {
            await Task.Delay(retryPolicy.Delay(retryCount), cancellationToken).ConfigureAwait(false);
            return await producer().ConfigureAwait(false);
        };
}
#endif
