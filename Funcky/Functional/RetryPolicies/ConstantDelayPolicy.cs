using System;

namespace Funcky
{
    public class ConstantDelayPolicy : IRetryPolicy
    {
        private readonly TimeSpan _delay;

        public ConstantDelayPolicy(int maxRetry, TimeSpan delay)
        {
            MaxRetry = maxRetry;
            _delay = delay;
        }

        public int MaxRetry { get; }

        public TimeSpan Duration(int onRetryCount) => _delay;
    }
}
