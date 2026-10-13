using Funcky.Internal.Validators;

namespace Funcky.RetryPolicies;

public sealed class LinearBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay)
    : IRetryPolicy
{
    private readonly TimeSpan _firstDelay = DelayValidator.Validate(firstDelay, nameof(firstDelay));

    public int MaxRetries { get; } = MaxRetriesValidator.Validate(maxRetries);

    public TimeSpan Delay(int retryCount) => _firstDelay.Multiply(retryCount);
}
