namespace Funcky.RetryPolicies;

public class ConstantDelayPolicy(int maxRetries, TimeSpan delay) : IRetryPolicy
{
    public int MaxRetries { get; } = maxRetries;

    public TimeSpan Delay(int retryCount) => delay;
}
