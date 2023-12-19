#pragma warning disable SA1010 // StyleCop support for collection expressions is missing

using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

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
