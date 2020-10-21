using System;
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
        public void OptionIsGreaterThanNull()
        {
            Assert.True(Option.Some(10).CompareTo(null) > 0);
        }

        [Fact]
        public void CompareToThrowsWhenRhsIsNotAnOption()
        {
            var option = Option.Some(10);
            Assert.Throws<ArgumentException>(() => _ = option.CompareTo("hello"));
        }

        [Fact]
        public void CompareToThrowsWhenRhsIsOptionWithDifferentItemType()
        {
            var optionOfInt = Option.Some(10);
            var optionOfString = Option.Some("hello");
            Assert.Throws<ArgumentException>(() => _ = optionOfInt.CompareTo(optionOfString));
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

        [Fact]
        public void CompareToThrowsWhenTwoSomeValuesWhereItemTypeIsNotComparableAreCompared()
        {
            var optionOne = Option.Some(new Person());
            var optionTwo = Option.Some(new Person());
            Assert.Throws<ArgumentException>(() => _ = optionOne.CompareTo(optionTwo));
        }

        [Fact]
        public void CompareToDoesNotThrowWhenTwoNoneValuesWhereItemTypeIsNotComparableAreCompared()
        {
            var optionOne = Option<Person>.None();
            var optionTwo = Option<Person>.None();
            _ = optionOne.CompareTo(optionTwo);
        }

        [Fact]
        public void CompareToDoesNotThrowWhenNoneValueAndSomeValueWhereItemTypeIsNotComparableAreCompared()
        {
            var optionOne = Option.Some(new Person());
            var optionTwo = Option<Person>.None();
            _ = optionOne.CompareTo(optionTwo);
            _ = optionTwo.CompareTo(optionOne);
        }

        private sealed class Person
        {
        }
    }
}
