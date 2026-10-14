using Funcky.Internal.Validators;

namespace Funcky.RetryPolicies;

public sealed class ExponentialBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay) : IRetryPolicy
{
    private const double BaseFactor = 1.5;

    private readonly TimeSpan _firstDelay = DelayValidator.Validate(firstDelay, nameof(firstDelay));

    public int MaxRetries { get; } = MaxRetriesValidator.Validate(maxRetries);

    public TimeSpan Delay(int retryCount)
        => _firstDelay.Multiply(Exponential(retryCount));

    private static double Exponential(int retryCount)
        => Math.Pow(BaseFactor, retryCount - 1);
}
