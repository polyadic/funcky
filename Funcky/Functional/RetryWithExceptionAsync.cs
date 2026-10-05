#if ASYNC_SUPPORTED
using Funcky.RetryPolicies;
using static Funcky.ValueTaskFactory;

namespace Funcky;

public static partial class AsyncFunctional
{
    /// <summary>Retries a producer as long as an exception matching the <paramref name="shouldRetry"/> predicate is thrown.
    /// When all retries are exhausted, the exception is propagated to the caller.</summary>
    /// <remarks>Once the <paramref name="cancellationToken"/> is cancelled no further attempt is made, even if the exception of the last attempt matches <paramref name="shouldRetry"/>.</remarks>
    public static ValueTask<TResult> RetryAsync<TResult>(
        Func<TResult> producer,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
        => RetryAwaitAsync(_ => ValueTaskFromResult(producer()), shouldRetry, retryPolicy, cancellationToken);

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    public static async ValueTask RetryAsync(
        Action action,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
        => await RetryAsync(ActionToUnit(action), shouldRetry, retryPolicy, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    public static ValueTask<TResult> RetryAwaitAsync<TResult>(
        Func<ValueTask<TResult>> producer,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
        => RetryAwaitAsync(_ => producer(), shouldRetry, retryPolicy, cancellationToken);

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    /// <remarks>The <paramref name="producer"/> receives the <paramref name="cancellationToken"/>, so it can abort an attempt that is in progress.</remarks>
    public static async ValueTask<TResult> RetryAwaitAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> producer,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        var retryCount = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await producer(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested && shouldRetry(exception) && retryCount < retryPolicy.MaxRetries)
            {
                retryCount++;
                await Task.Delay(retryPolicy.Delay(retryCount), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    public static ValueTask RetryAwaitAsync(
        Func<ValueTask> action,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
        => RetryAwaitAsync(_ => action(), shouldRetry, retryPolicy, cancellationToken);

    /// <inheritdoc cref="RetryAsync{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy,System.Threading.CancellationToken)"/>
    /// <remarks>The <paramref name="action"/> receives the <paramref name="cancellationToken"/>, so it can abort an attempt that is in progress.</remarks>
    public static async ValueTask RetryAwaitAsync(
        Func<CancellationToken, ValueTask> action,
        Func<Exception, bool> shouldRetry,
        IRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        await RetryAwaitAsync(Producer, shouldRetry, retryPolicy, cancellationToken).ConfigureAwait(false);

        async ValueTask<Unit> Producer(CancellationToken token)
        {
            await action(token).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
#endif
