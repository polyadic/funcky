using Funcky.RetryPolicies;
using static Funcky.Async.ValueTaskFactory;

namespace Funcky;

public static partial class AsyncFunctional
{
    /// <summary>Retries a producer as long as an exception matching the <paramref name="shouldRetry"/> predicate is thrown.
    /// When all retries are exhausted, the exception is propagated to the caller.</summary>
    public static ValueTask<TResult> RetryAsync<TResult>(
        Func<TResult> producer,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
        => RetryAwaitAsync(() => ValueTaskFromResult(producer()), shouldRetry, retryPolicy, cancellationToken);

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    public static async ValueTask<TResult> RetryAwaitAsync<TResult>(
        Func<ValueTask<TResult>> producer,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        var retryCount = 1;
        while (true)
        {
            try
            {
                return await producer().ConfigureAwait(false);
            }
            catch (Exception exception) when (shouldRetry(exception) && retryCount <= retryPolicy.MaxRetries)
            {
                await Task.Delay(retryPolicy.Delay(retryCount), cancellationToken).ConfigureAwait(false);
                retryCount++;
            }
        }
    }
}
