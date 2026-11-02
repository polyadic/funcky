#if ASYNC_SUPPORTED
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class InspectTest
{
    [Fact]
    public void InspectIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.Inspect(NoOperation);
    }

    [Fact]
    public async Task GivenAnAsyncEnumerableAndInjectWeCanApplySideEffectsToAsyncEnumerables()
    {
        var sideEffect = 0;
        var numbers = AsyncSequence.Return(1, 2, 3, 42);

        var numbersWithSideEffect = numbers
            .Inspect(n => { ++sideEffect; });

        Assert.Equal(0, sideEffect);

        await numbersWithSideEffect.ToListAsync();

        Assert.Equal(await numbers.CountAsync(), sideEffect);
    }

    [Fact]
    public async Task CancellationIsPropagated()
    {
        await AsyncAssert.CancellationIsPropagated(new AssertIsCancellationRequestedAsyncSequence<Unit>().Inspect(NoOperation));
    }
}
#endif
