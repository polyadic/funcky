using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class IntersperseTest
{
    [Fact]
    public void IntersperseIsEvaluatedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<int>();
        _ = doNotEnumerate.Intersperse(42);
    }

    [Fact]
    public async Task InterspersingAnEmptyEnumerableReturnsAnEmptyEnumerable()
    {
        Assert.False(await AsyncEnumerable.Empty<int>().Intersperse(42).AnyAsync());
    }

    [Fact]
    public async Task InterspersingASequenceWithOneElementReturnsOriginalSequence()
    {
        var source = Sequence.Return(10).ToAsyncEnumerable();
        Assert.True(await source.SequenceEqualAsync(source.Intersperse(42)));
    }

    [Theory]
    [InlineData(new[] { 1, 0, 2 }, new[] { 1, 2 })]
    [InlineData(new[] { 1, 0, 2, 0, 3 }, new[] { 1, 2, 3 })]
    [InlineData(new[] { 1, 0, 2, 0, 3, 0, 4 }, new[] { 1, 2, 3, 4 })]
    public async Task InterspersingASequenceWithMoreThanOneElementReturnsExpectedSequence(IEnumerable<int> expected, IEnumerable<int> source)
    {
        Assert.True(await expected.ToAsyncEnumerable().SequenceEqualAsync(source.ToAsyncEnumerable().Intersperse(0)));
    }
}
