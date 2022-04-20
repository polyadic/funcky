using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Fact]
    public void FromBooleanReturnsAMatchingOptionUnit()
    {
        FunctionalAssert.IsNone(Option.FromBoolean(false));
        FunctionalAssert.IsSome(Unit.Value, Option.FromBoolean(true));
    }

    [Fact]
    public void FromBooleanWithValueReturnsAMatchingOptionValue()
    {
        var expectedValue = 1337;

        FunctionalAssert.IsNone(Option.FromBoolean(false, expectedValue));
        FunctionalAssert.IsSome(expectedValue, Option.FromBoolean(true, expectedValue));
    }

    [Fact]
    public void FromBooleanWithASelectorReturnsAMatchingOptionValue()
    {
        var expectedValue = 1337;

        FunctionalAssert.IsNone(Option.FromBoolean(false, () => expectedValue));
        FunctionalAssert.IsSome(expectedValue, Option.FromBoolean(true, () => expectedValue));
    }

    [Fact]
    public void TheBooleanSelectorIsLazyAndOnlyCalledWhenTheTrue()
    {
        FunctionalAssert.IsNone(Option.FromBoolean(false, FailOnCall.Function<string>));
    }
}
