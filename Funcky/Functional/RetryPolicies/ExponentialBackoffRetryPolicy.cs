using System;

namespace Funcky
{
    public sealed class ExponentialBackoffRetryPolicy : IRetryPolicy
    {
        private const double BaseFactor = 1.5;
        private readonly TimeSpan _firstDelay;

        public ExponentialBackoffRetryPolicy(int maxRetry, TimeSpan firstDelay)
        {
            MaxRetries = maxRetry;
            _firstDelay = firstDelay;
        }

        public int MaxRetries { get; }

#if TIMESPAN_MULTIPLY_SUPPORTED
        public TimeSpan Duration(int onRetryCount)
            => _firstDelay * Exponential(onRetryCount);
#else
        public TimeSpan Duration(int onRetryCount)
            => TimeSpan.FromTicks((long)(_firstDelay.Ticks * Exponential(onRetryCount)));
#endif

        private double Exponential(int onRetryCount) => Math.Pow(BaseFactor, onRetryCount);
    }
}
