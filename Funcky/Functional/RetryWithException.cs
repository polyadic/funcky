using Funcky.RetryPolicies;

namespace Funcky;

public static partial class Functional
{
    /// <summary>Retries a producer as long as an exception matching the <paramref name="shouldRetry"/> predicate is thrown.
    /// When all retries are exhausted, the exception is propagated to the caller.</summary>
    /// <remarks>Note that this function uses <see cref="Thread.Sleep(TimeSpan)"/> for the delay.
    /// Consider using <c>RetryAsync</c> from the <c>Funcky</c> package instead.</remarks>
    public static TResult Retry<TResult>(Func<TResult> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy)
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                return producer();
            }
            catch (Exception exception) when (shouldRetry(exception) && retryCount < retryPolicy.MaxRetries)
            {
                retryCount++;
                Thread.Sleep(retryPolicy.Delay(retryCount));
            }
        }
    }

    /// <inheritdoc cref="Retry{TResult}(System.Func{TResult},System.Func{System.Exception,bool},Funcky.RetryPolicies.IRetryPolicy)"/>
    public static void Retry(Action action, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy)
        => Retry(ActionToUnit(action), shouldRetry, retryPolicy);
}
