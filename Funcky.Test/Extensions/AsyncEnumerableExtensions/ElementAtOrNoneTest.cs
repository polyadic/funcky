#if ASYNC_SUPPORTED
using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class ElementAtOrNoneTest
{
    [Theory]
    [InlineData(-42)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public async Task ElementAtOrNoneReturnsAlwaysANoneOnAnEmptyEnumerable(int index)
    {
        Assert.None(await EmptyEnumerable.ElementAtOrNoneAsync(index));
    }

    [Fact]
    public async Task ElementAtOrNoneReturnsSomeWithinTheRangeAndNoneOutside()
    {
        Assert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-10));
        Assert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-1));
        Assert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(0));
        Assert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(1));
        Assert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(2));
        Assert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(3));
        Assert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(5));
        Assert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(10));
    }

    public sealed class IndexIndex
    {
        [FunckyProperty(Verbose = true)]
        public Property BehavesIdenticalToSynchronousCounterpart(List<int> source, Index index)
            => (source.ElementAtOrNone(index) == source.ToAsyncEnumerable().ElementAtOrNoneAsync(index).Result)
                .ToProperty();
    }
}
#endif
