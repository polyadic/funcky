using System.Collections.Immutable;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Monads
{
    public sealed partial class OptionTest
    {
        [Fact]
        public void NoneIsLessThanSome()
        {
            Assert.True(Option<int>.None() < Option.Some(10));
            Assert.True(Option<int>.None() <= Option.Some(10));
        }

        [Fact]
        public void SomeIsGreaterThanNone()
        {
            Assert.True(Option.Some(10) > Option<int>.None());
            Assert.True(Option.Some(10) >= Option<int>.None());
        }

        [Fact]
        public void GreaterSomeValueIsGreaterThanSomeValue()
        {
            Assert.True(Option.Some(20) > Option.Some(10));
            Assert.True(Option.Some(20) >= Option.Some(10));
        }

        [Fact]
        public void LessSomeValueIsLessThanSomeValue()
        {
            Assert.True(Option.Some(10) < Option.Some(20));
            Assert.True(Option.Some(10) <= Option.Some(20));
        }

        [Fact]
        public void CompareToReturnsZeroWhenValuesAreEqual()
        {
            Assert.Equal(0, Option.Some(10).CompareTo(Option.Some(10)));
        }

        [Fact]
        public void CompareToReturnsZeroWhenBothAreNone()
        {
            Assert.Equal(0, Option<string>.None().CompareTo(Option<string>.None()));
        }

        [Fact]
        public void EnumerableOfOptionsCanBeSorted()
        {
            var unsorted = ImmutableList.Create(
                Option.Some(5),
                Option<int>.None(),
                Option.Some(0),
                Option.Some(10),
                Option<int>.None());
            var expected = ImmutableList.Create(
                Option<int>.None(),
                Option<int>.None(),
                Option.Some(0),
                Option.Some(5),
                Option.Some(10));
            var sorted = unsorted.Sort();
            Assert.Equal(expected, sorted);
        }
    }
}
