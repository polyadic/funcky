using Funcky.Internal.Validators;

namespace Funcky.RetryPolicies;

public class ConstantDelayPolicy(int maxRetries, TimeSpan delay) : IRetryPolicy
{
    public int MaxRetries { get; } = MaxRetriesValidator.Validate(maxRetries);

    public TimeSpan Delay(int retryCount) => delay;
}
