namespace Funcky.Test.Monads
{
    public sealed partial class OptionTest
    {
        [Fact]
        public void FromBooleanReturnsAMatchingOptionUnit()
        {
            FunctionalAssert.IsNone(Option.FromBoolean(false));
            var value = FunctionalAssert.IsSome(Option.FromBoolean(true));

            Assert.Equal(Unit.Value, value);
        }

        [Fact]
        public void FromBooleanWithValueReturnsAMatchingOptionValue()
        {
            var expectedValue = 1337;

            FunctionalAssert.IsNone(Option.FromBoolean(false, expectedValue));
            var value = FunctionalAssert.IsSome(Option.FromBoolean(true, expectedValue));

            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void FromBooleanWithASelectorReturnsAMatchingOptionValue()
        {
            var expectedValue = 1337;

            FunctionalAssert.IsNone(Option.FromBoolean(false, () => expectedValue));
            var value = FunctionalAssert.IsSome(Option.FromBoolean(true, () => expectedValue));

            Assert.Equal(expectedValue, value);
        }
    }
}
