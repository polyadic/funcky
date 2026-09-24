#if INTEGRATED_ASYNC
using FsCheck.Xunit;

namespace Funcky.Async.Test;

internal sealed class FunckyAsyncPropertyAttribute : PropertyAttribute
{
    public FunckyAsyncPropertyAttribute()
    {
        Arbitrary = [typeof(AsyncGenerator)];
    }
}
#endif
