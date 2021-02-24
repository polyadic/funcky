using System;

namespace Funcky.RetryPolicies
{
    public sealed class LinearBackoffRetryPolicy : IRetryPolicy
    {
        private readonly TimeSpan _firstDelay;

        public LinearBackoffRetryPolicy(int maxRetry, TimeSpan firstDelay)
        {
            MaxRetries = maxRetry;
            _firstDelay = firstDelay;
        }

        public int MaxRetries { get; }

        public TimeSpan Duration(int onRetryCount) => _firstDelay.Multiply(onRetryCount);
    }
}
