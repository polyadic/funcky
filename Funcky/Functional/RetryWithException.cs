using Funcky.RetryPolicies;

namespace Funcky;

public static partial class Functional
{
    /// <summary>Retries a producer as long as an exception matching the <paramref name="shouldRetry"/> predicate is thrown.
    /// When all retries are exhausted, the exception is propagated to the caller.</summary>
    /// <remarks>Note that this function uses <see cref="Thread.Sleep(TimeSpan)"/> for the delay.
    /// Consider using <c>RetryAsync</c> from the <c>Funcky.Async</c> package instead.</remarks>
    public static TResult Retry<TResult>(Func<TResult> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy)
    {
        var retryCount = 1;
        while (true)
        {
            try
            {
                return producer();
            }
            catch (Exception exception) when (shouldRetry(exception) && retryCount <= retryPolicy.MaxRetries)
            {
                Thread.Sleep(retryPolicy.Delay(retryCount));
                retryCount++;
            }
        }
    }
}
