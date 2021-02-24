using System;

namespace Funcky.RetryPolicies
{
    public interface IRetryPolicy
    {
        int MaxRetries { get; }

        TimeSpan Duration(int onRetryCount);
    }
}
