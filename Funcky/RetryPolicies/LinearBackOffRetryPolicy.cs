namespace Funcky.RetryPolicies;

public sealed class LinearBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay)
    : IRetryPolicy
{
    public int MaxRetries => maxRetries;

    public TimeSpan Delay(int retryCount) => firstDelay.Multiply(retryCount);
}
