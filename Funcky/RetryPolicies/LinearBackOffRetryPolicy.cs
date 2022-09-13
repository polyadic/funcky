namespace Funcky.RetryPolicies;

public sealed class LinearBackOffRetryPolicy
    : IRetryPolicy
{
    private readonly TimeSpan _firstDelay;

    public LinearBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay)
        => (MaxRetries, _firstDelay) = (maxRetries, firstDelay);

    public int MaxRetries { get; }

    public TimeSpan Delay(int retryCount) => _firstDelay.Multiply(retryCount);
}
