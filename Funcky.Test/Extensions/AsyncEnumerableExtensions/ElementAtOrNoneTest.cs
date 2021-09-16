using Funcky.Xunit;
using Xunit;
using static Funcky.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions
{
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
            FunctionalAssert.IsNone(await EmptyEnumerable.ElementAtOrNoneAsync(index));
        }

        [Fact]
        public async Task ElementAtOrNoneReturnsSomeWithinTheRangeAndNoneOutside()
        {
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-10));
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(-1));
            FunctionalAssert.IsSome(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(0));
            FunctionalAssert.IsSome(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(1));
            FunctionalAssert.IsSome(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(2));
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(3));
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(5));
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.ElementAtOrNoneAsync(10));
        }
    }
}
