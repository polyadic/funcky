namespace Funcky.RetryPolicies;

public sealed class DoNotRetryPolicy : IRetryPolicy
{
    public int MaxRetries => 0;

    public TimeSpan Delay(int retryCount) => TimeSpan.Zero;
}
