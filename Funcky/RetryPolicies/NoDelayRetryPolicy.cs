namespace Funcky.RetryPolicies;

public sealed class NoDelayRetryPolicy : ConstantDelayPolicy
{
    public NoDelayRetryPolicy(int maxRetries)
        : base(maxRetries, TimeSpan.Zero)
    {
    }
}
