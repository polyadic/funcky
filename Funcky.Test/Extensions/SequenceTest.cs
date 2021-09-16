using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class SequenceTest
    {
        [Fact]
        public void GivenAnObjectWeCreateAnIEnumerableWithToEnumerable()
        {
            AcceptIntegers(Sequence.Return(42));

            Unit unit;
            AcceptUnits(Sequence.Return(unit));
        }

        [Fact]
        public void ToEnumerableReturnsEmptyEnumerableWhenReferenceIsNull()
        {
            string? value = null;
            Assert.Empty(Sequence.FromNullable(value));
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenReferenceIsNotNull()
        {
            const string value = "foo";
            Assert.Single(Sequence.FromNullable(value), value);
        }

        [Fact]
        public void ToEnumerableReturnsEmptyEnumerableWhenNullableValueTypeIsNull()
        {
            int? value = null;
            Assert.Empty(Sequence.FromNullable(value));
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenNullableValueTypeIsNotNull()
        {
            int? value = 10;
            Assert.Single(Sequence.FromNullable(value), value);
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenValueTypeIsNotNull()
        {
            const int value = 10;
            Assert.Single(Sequence.Return(value), value);
        }

        private static void AcceptIntegers(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Assert.Equal(42, value);
            }
        }

        private static void AcceptUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                Assert.Equal(default, unit);
            }
        }
    }
}
