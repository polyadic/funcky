using FsCheck;
using FsCheck.Xunit;
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
        FunctionalAssert.None(await EmptyEnumerable.ElementAtOrNoneAsync(index));
    }

    [Fact]
    public async Task ElementAtOrNoneReturnsSomeWithinTheRangeAndNoneOutside()
    {
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-10));
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-1));
        FunctionalAssert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(0));
        FunctionalAssert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(1));
        FunctionalAssert.Some(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(2));
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(3));
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(5));
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(10));
    }

    public sealed class IndexIndex
    {
        public IndexIndex() => FunckyGenerators.Register();

        [Property(Verbose = true)]
        public Property BehavesIdenticalToSynchronousCounterpart(List<int> source, Index index)
            => (source.ElementAtOrNone(index) == source.ToAsyncEnumerable().ElementAtOrNoneAsync(index).Result)
                .ToProperty();
    }
}
