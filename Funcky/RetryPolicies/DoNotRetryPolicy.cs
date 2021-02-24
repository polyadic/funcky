using System;

namespace Funcky.RetryPolicies
{
    public sealed class DoNotRetryPolicy : IRetryPolicy
    {
        public int MaxRetries => 0;

        public TimeSpan Duration(int onRetryCount) => TimeSpan.Zero;
    }
}
