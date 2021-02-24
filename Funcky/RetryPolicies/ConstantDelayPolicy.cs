using System;

namespace Funcky.RetryPolicies
{
    public class ConstantDelayPolicy : IRetryPolicy
    {
        private readonly TimeSpan _delay;

        public ConstantDelayPolicy(int maxRetry, TimeSpan delay)
        {
            MaxRetries = maxRetry;
            _delay = delay;
        }

        public int MaxRetries { get; }

        public TimeSpan Duration(int onRetryCount) => _delay;
    }
}
