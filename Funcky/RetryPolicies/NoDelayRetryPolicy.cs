namespace Funcky.RetryPolicies;

public sealed class NoDelayRetryPolicy : ConstantDelayPolicy
{
    public NoDelayRetryPolicy(int maxRetry)
        : base(maxRetry, TimeSpan.Zero)
    {
    }
}
