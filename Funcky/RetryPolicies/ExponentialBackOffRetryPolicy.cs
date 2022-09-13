namespace Funcky.RetryPolicies;

public sealed class ExponentialBackOffRetryPolicy : IRetryPolicy
{
    private const double BaseFactor = 1.5;
    private readonly TimeSpan _firstDelay;

    public ExponentialBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay)
        => (MaxRetries, _firstDelay) = (maxRetries, firstDelay);

    public int MaxRetries { get; }

    public TimeSpan Delay(int retryCount)
        => _firstDelay.Multiply(Exponential(retryCount));

    private static double Exponential(int retryCount)
        => Math.Pow(BaseFactor, retryCount);
}
