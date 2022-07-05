using Funcky.RetryPolicies;

namespace Funcky;

public static partial class AsyncFunctional
{
    /// <summary>
    /// Calls the given <paramref name="producer"/> over and over until it returns a value.
    /// </summary>
    public static async ValueTask<TResult> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await (await producer().ConfigureAwait(false)).Match(
            none: (Func<ValueTask<TResult>>)(async () => await RetryAsync(producer, cancellationToken).ConfigureAwait(false)),
            some: result => new ValueTask<TResult>(result)).ConfigureAwait(false);
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

    private static IAsyncEnumerable<Option<TResult>> TailRetriesAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        where TResult : notnull
        => Retries(retryPolicy)
            .SelectAwait(ProduceDelayedAsync(producer, retryPolicy, cancellationToken));

    private static IAsyncEnumerable<int> Retries(IRetryPolicy retryPolicy)
        => AsyncEnumerable.Range(0, retryPolicy.MaxRetries);

    private static Func<int, ValueTask<Option<TResult>>> ProduceDelayedAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        where TResult : notnull
        => async retryCount =>
        {
            await Task.Delay(retryPolicy.Duration(retryCount), cancellationToken).ConfigureAwait(false);
            return await producer().ConfigureAwait(false);
        };
}
