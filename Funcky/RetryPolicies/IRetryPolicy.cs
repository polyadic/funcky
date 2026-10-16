namespace Funcky.RetryPolicies;

public interface IRetryPolicy
{
    /// <summary>
    /// The maximum number of retries after the initial attempt.
    /// </summary>
    int MaxRetries { get; }

    /// <summary>
    /// Returns the delay to wait before the given retry.
    /// </summary>
    /// <param name="retryCount">The one-based number of the retry: <c>1</c> is the first retry after the initial attempt, <see cref="MaxRetries"/> is the last.</param>
    TimeSpan Delay(int retryCount);
}
