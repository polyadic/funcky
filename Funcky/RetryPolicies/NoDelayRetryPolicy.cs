namespace Funcky.RetryPolicies;

public sealed class NoDelayRetryPolicy(int maxRetries) : ConstantDelayPolicy(maxRetries, TimeSpan.Zero);
