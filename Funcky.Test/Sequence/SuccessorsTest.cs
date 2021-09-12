using System.Linq;
using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public sealed class SuccessorsTest
    {
        [Fact]
        public void ReturnsEmptySequenceWhenFirstItemIsNone()
        {
            Assert.Empty(Sequence.Successors(Option<int>.None(), Identity));
        }

        [Fact]
        public void ReturnsOnlyTheFirstItemWhenSuccessorFunctionImmediatelyReturnsNone()
        {
            var first = Assert.Single(Sequence.Successors(10, _ => Option<int>.None()));
            Assert.Equal(10, first);
        }

        [Fact]
        public void SuccessorsWithNonOptionFunctionReturnsEndlessEnumerable()
        {
            const int count = 40;
            Assert.Equal(count, Sequence.Successors(0, Identity).Take(count).Count());
        }

        [Fact]
        public void SuccessorsReturnsEnumerableThatReturnsValuesBasedOnSeed()
        {
            Assert.Equal(
                Enumerable.Range(0, 10),
                Sequence.Successors(0, i => i + 1).Take(10));
        }

        [Fact]
        public void SuccessorsReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFunc()
        {
            Assert.Equal(
                Enumerable.Range(0, 11),
                Sequence.Successors(0, i => i < 10 ? i + 1 : Option<int>.None()));
        }
    }
}
