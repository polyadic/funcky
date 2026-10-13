using Funcky.Internal.Validators;

namespace Funcky.RetryPolicies;

public sealed class LinearBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay)
    : IRetryPolicy
{
    public int MaxRetries { get; } = MaxRetriesValidator.Validate(maxRetries);

    public TimeSpan Delay(int retryCount) => firstDelay.Multiply(retryCount);
}
