#if !NET48

using FsCheck.Xunit;

namespace Funcky.Test.Async;

internal sealed class FunckyAsyncPropertyAttribute : PropertyAttribute
{
    public FunckyAsyncPropertyAttribute()
    {
        Arbitrary = [typeof(AsyncGenerator)];
    }
}

#endif
