using System.Linq;
using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public sealed class SequenceTest
    {
        [Fact]
        public void GenerateWithFunctionThatImmediatelyReturnsNoneReturnsAnEmptyEnumerable()
        {
            Assert.Empty(Sequence.Generate(0, _ => Option<int>.None()));
        }

        [Fact]
        public void GenerateWithNonOptionFunctionReturnsEndlessEnumerable()
        {
            const int count = 40;
            Assert.Equal(count, Sequence.Generate(0, Identity).Take(count).Count());
        }

        [Fact]
        public void GenerateReturnsEnumerableThatReturnsValuesBasedOnSeed()
        {
            Assert.Equal(
                Enumerable.Range(1, 10),
                Sequence.Generate(0, i => i + 1).Take(10));
        }

        [Fact]
        public void GenerateReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFunc()
        {
            Assert.Equal(
                Enumerable.Range(1, 10),
                Sequence.Generate(0, i => i < 10 ? Option.Some(i + 1) : Option<int>.None()));
        }
    }
}
