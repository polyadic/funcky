using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class SlidingWindowTest
{
    [Fact]
    public void ASlidingWindowIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.SlidingWindow(42);
    }

    [Property]
    public void ASlidingWindowFromAnEmptySequenceIsAlwaysEmpty(PositiveInt width)
        => AsyncEnumerable
            .Empty<int>()
            .SlidingWindow(width.Get)
            .NoneAsync()
            .Result
            .ToProperty();

    [Fact]
    public async Task GivenASourceSequenceEqualInLengthToTheSlidingWindowWidthReturnsASequenceWithOneElementAsync()
    {
        var source = AsyncEnumerable.Range(0, 5);

        Assert.Equal(1, await source.SlidingWindow(5).CountAsync());
        Assert.Equal(Enumerable.Range(0, 5), await source.SlidingWindow(5).SingleAsync());
    }

    [Fact]
    public async Task GivenASourceSequenceShorterThanTheSlidingWindowWidthReturnsAnEmptySequence()
    {
        var source = AsyncEnumerable.Range(0, 5);

        await AsyncAssert.Empty(source.SlidingWindow(10));
    }

    [Fact]
    public async Task SlidingWindowReturnsTheCorrectAmountOfWindowsAllOfTheSameSizeAsync()
    {
        const int width = 5;
        var source = AsyncEnumerable.Range(0, 10);
        var windows = source.SlidingWindow(width);

        Assert.Equal(6, await windows.CountAsync());
        await foreach (var window in windows)
        {
            Assert.Equal(width, window.Count);
        }
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-42)]
    [InlineData(-1)]
    [InlineData(0)]
    public void SlidingWindowThrowsOnNonPositiveWidth(int width)
    {
        var source = AsyncEnumerable.Range(0, 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => source.SlidingWindow(width));
    }

    [Fact]
    public async Task SlidingWindowReturnsASequenceOfConsecutiveWindowsAsync()
    {
        const int width = 4;
        var source = AsyncEnumerable.Range(0, 6);

        await AsyncAssert.Collection(
            source.SlidingWindow(width),
            window => Assert.Equal(Enumerable.Range(0, width), window),
            window => Assert.Equal(Enumerable.Range(1, width), window),
            window => Assert.Equal(Enumerable.Range(2, width), window));
    }

    [Fact]
    public async Task CancellationIsPropagated()
    {
        var canceledToken = new CancellationToken(canceled: true);
        _ = await new AssertIsCancellationRequestedAsyncSequence<Unit>().SlidingWindow(1).ToListAsync(canceledToken);
    }
}
