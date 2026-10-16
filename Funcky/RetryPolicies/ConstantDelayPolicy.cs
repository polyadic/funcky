using Funcky.Internal.Validators;

namespace Funcky.RetryPolicies;

public class ConstantDelayPolicy(int maxRetries, TimeSpan delay) : IRetryPolicy
{
    private readonly TimeSpan _delay = DelayValidator.Validate(delay, nameof(delay));

    public int MaxRetries { get; } = MaxRetriesValidator.Validate(maxRetries);

    public TimeSpan Delay(int retryCount) => _delay;
}
