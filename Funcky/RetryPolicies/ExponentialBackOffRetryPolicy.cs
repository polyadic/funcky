namespace Funcky.RetryPolicies;

public sealed class ExponentialBackOffRetryPolicy(int maxRetries, TimeSpan firstDelay) : IRetryPolicy
{
    private const double BaseFactor = 1.5;

    public int MaxRetries => maxRetries;

    public TimeSpan Delay(int retryCount)
        => firstDelay.Multiply(Exponential(retryCount));

    private static double Exponential(int retryCount)
        => Math.Pow(BaseFactor, retryCount);
}
