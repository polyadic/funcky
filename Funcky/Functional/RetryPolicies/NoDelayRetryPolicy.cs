using System;

namespace Funcky
{
    public class NoDelayRetryPolicy : ConstantDelayPolicy
    {
        public NoDelayRetryPolicy(int maxRetry)
            : base(maxRetry, TimeSpan.Zero)
        {
        }
    }
}
