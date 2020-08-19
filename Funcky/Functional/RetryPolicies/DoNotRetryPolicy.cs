using System;

namespace Funcky
{
    public sealed class DoNotRetryPolicy : IRetryPolicy
    {
        public int MaxRetry => 0;

        public TimeSpan Duration(int onRetryCount) => TimeSpan.Zero;
    }
}
