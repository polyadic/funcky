namespace Funcky.RetryPolicies;

public interface IRetryPolicy
{
    int MaxRetries { get; }

    TimeSpan Delay(int retryCount);
}
