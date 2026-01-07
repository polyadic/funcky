using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

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
        var source = AsyncSequence.Return(10);
        Assert.True(await source.SequenceEqualAsync(source.Intersperse(42)));
    }

    [Theory]
    [MemberData(nameof(ValueReferenceEnumerables))]
    public async Task InterspersingASequenceWithMoreThanOneElementReturnsExpectedSequence(IAsyncEnumerable<int> expected, IAsyncEnumerable<int> source)
    {
        Assert.True(await expected.SequenceEqualAsync(source.Intersperse(0)));
    }

    public static TheoryData<IAsyncEnumerable<int>, IAsyncEnumerable<int>> ValueReferenceEnumerables()
        => new()
        {
            { AsyncSequence.Return(1, 0, 2), AsyncSequence.Return(1, 2) },
            { AsyncSequence.Return(1, 0, 2, 0, 3), AsyncSequence.Return(1, 2, 3) },
            { AsyncSequence.Return(1, 0, 2, 0, 3, 0, 4), AsyncSequence.Return(1, 2, 3, 4) },
        };
}
