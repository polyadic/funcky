using System;

namespace Funcky
{
    public interface IRetryPolicy
    {
        int MaxRetry { get; }

        TimeSpan Duration(int onRetryCount);
    }
}
