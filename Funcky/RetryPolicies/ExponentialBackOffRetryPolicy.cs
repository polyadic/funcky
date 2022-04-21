namespace Funcky.RetryPolicies;

public sealed class ExponentialBackOffRetryPolicy : IRetryPolicy
{
    private const double BaseFactor = 1.5;
    private readonly TimeSpan _firstDelay;

    public ExponentialBackOffRetryPolicy(int maxRetry, TimeSpan firstDelay)
        => (MaxRetries, _firstDelay) = (maxRetry, firstDelay);

    public int MaxRetries { get; }

    public TimeSpan Duration(int onRetryCount)
        => _firstDelay.Multiply(Exponential(onRetryCount));

    private static double Exponential(int onRetryCount)
        => Math.Pow(BaseFactor, onRetryCount);
}
