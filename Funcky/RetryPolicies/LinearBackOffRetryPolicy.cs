namespace Funcky.RetryPolicies
{
    public sealed class LinearBackOffRetryPolicy
        : IRetryPolicy
    {
        private readonly TimeSpan _firstDelay;

        public LinearBackOffRetryPolicy(int maxRetry, TimeSpan firstDelay)
            => (MaxRetries, _firstDelay) = (maxRetry, firstDelay);

        public int MaxRetries { get; }

        public TimeSpan Duration(int onRetryCount) => _firstDelay.Multiply(onRetryCount);
    }
}
