using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class PartitionTest
    {
        [Fact]
        public void ReturnsTwoEmptyEnumerablesWhenSourceIsEmpty()
        {
            var (evens, odds) = Enumerable.Empty<int>().Partition(IsEven);
            Assert.Empty(evens);
            Assert.Empty(odds);
        }

        [Fact]
        public void PartitionsItemsIntoTruesAndFalses()
        {
            var (evens, odds) = Enumerable.Range(0, 6).Partition(IsEven);
            Assert.Equal(new[] { 0, 2, 4 }, evens);
            Assert.Equal(new[] { 1, 3, 5 }, odds);
        }

        private static bool IsEven(int n) => n % 2 == 0;
    }
}
