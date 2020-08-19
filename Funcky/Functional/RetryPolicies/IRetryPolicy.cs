using System;

namespace Funcky
{
    public interface IRetryPolicy
    {
        int MaxRetries { get; }

        TimeSpan Duration(int onRetryCount);
    }
}
