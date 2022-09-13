namespace Funcky.RetryPolicies;

public class ConstantDelayPolicy : IRetryPolicy
{
    private readonly TimeSpan _delay;

    public ConstantDelayPolicy(int maxRetries, TimeSpan delay)
        => (MaxRetries, _delay) = (maxRetries, delay);

    public int MaxRetries { get; }

    public TimeSpan Delay(int retryCount) => _delay;
}
