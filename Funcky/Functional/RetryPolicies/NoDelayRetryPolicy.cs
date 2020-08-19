using System;

namespace Funcky
{
    public sealed class NoDelayRetryPolicy : ConstantDelayPolicy
    {
        public NoDelayRetryPolicy(int maxRetry)
            : base(maxRetry, TimeSpan.Zero)
        {
        }
    }
}
