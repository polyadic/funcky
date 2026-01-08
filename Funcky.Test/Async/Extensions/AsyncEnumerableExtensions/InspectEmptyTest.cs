using Funcky.Test.Async.TestUtilities;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

public sealed class InspectEmptyTest
{
    [Fact]
    public void InspectEmptyIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();
        _ = doNotEnumerate.InspectEmpty(NoOperation);
    }

    [Fact]
    public async Task InspectEmptyExecutesAnInspectionFunctionOnMaterializationOnAnEmptyEnumerable()
    {
        var sideEffect = 0;
        var asyncEnumerable = AsyncEnumerable.Empty<string>();

        _ = await asyncEnumerable.InspectEmpty(() => sideEffect = 1).MaterializeAsync();

        Assert.Equal(1, sideEffect);
    }

    [Fact]
    public void InspectEmptyExecutesNoInspectionFunctionOnMaterializationOnANonEmptyEnumerable()
    {
        var sideEffect = 0;
        var asyncEnumerable = AsyncSequence.Return("Hello", "World");

        _ = asyncEnumerable.InspectEmpty(() => sideEffect = 1).MaterializeAsync();

        Assert.Equal(0, sideEffect);
    }
}
