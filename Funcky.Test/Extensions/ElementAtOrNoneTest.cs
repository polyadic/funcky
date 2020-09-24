using System.Linq;
using Funcky.Extensions;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions
{
    public class ElementAtOrNoneTest
    {
        [Theory]
        [InlineData(-42)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        public void ElementAtOrNoneReturnsAlwaysANoneOnAnEmptyEnumerable(int index)
        {
            var sequence = Enumerable.Empty<int>();

            FunctionalAssert.IsNone(sequence.ElementAtOrNone(index));
        }

        [Fact]
        public void ElementAtOrNoneReturnsSomeWithinTheRangeAndNoneOutside()
        {
            var sequence = Enumerable.Range(1, 5);

            FunctionalAssert.IsNone(sequence.ElementAtOrNone(-10));
            FunctionalAssert.IsNone(sequence.ElementAtOrNone(-1));
            FunctionalAssert.IsSome(sequence.ElementAtOrNone(0));
            FunctionalAssert.IsSome(sequence.ElementAtOrNone(2));
            FunctionalAssert.IsSome(sequence.ElementAtOrNone(4));
            FunctionalAssert.IsNone(sequence.ElementAtOrNone(5));
            FunctionalAssert.IsNone(sequence.ElementAtOrNone(10));
        }
    }
}
