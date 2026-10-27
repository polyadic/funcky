#pragma warning disable CS0618 // FromBoolean is obsolete, it keeps working until 4.0.0
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Fact]
    public void FromBooleanReturnsAMatchingOptionUnit()
    {
        FunctionalAssert.None(Option.FromBoolean(false));
        FunctionalAssert.Some(Unit.Value, Option.FromBoolean(true));
    }

    [Fact]
    public void FromBooleanWithValueReturnsAMatchingOptionValue()
    {
        const int expectedValue = 1337;

        FunctionalAssert.None(Option.FromBoolean(false, expectedValue));
        FunctionalAssert.Some(expectedValue, Option.FromBoolean(true, expectedValue));
    }

    [Fact]
    public void FromBooleanWithASelectorReturnsAMatchingOptionValue()
    {
        var expectedValue = 1337;

        FunctionalAssert.None(Option.FromBoolean(false, () => expectedValue));
        FunctionalAssert.Some(expectedValue, Option.FromBoolean(true, () => expectedValue));
    }

    [Fact]
    public void TheBooleanSelectorIsLazyAndOnlyCalledWhenTheTrue()
    {
        FunctionalAssert.None(Option.FromBoolean(false, FailOnCall.Function<string>));
    }
}
