using System.Linq;
using Funcky.Extensions;
using Funcky.Test.TestUtils;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class PartitionTest
    {
        [Fact(Skip="TODO fix")]
        public void PartitionIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.Partition(True);
        }

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

        [Fact]
        public void RightItemsAreEmptyWhenPredicateMatchesAllItems()
        {
            var (left, right) = Enumerable.Range(0, 6).Partition(True);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, left);
            Assert.Empty(right);
        }

        [Fact]
        public void LeftItemsAreEmptyWhenPredicateMatchesNoItems()
        {
            var (left, right) = Enumerable.Range(0, 6).Partition(False);
            Assert.Empty(left);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, right);
        }

        private static bool IsEven(int n) => n % 2 == 0;
    }
}
