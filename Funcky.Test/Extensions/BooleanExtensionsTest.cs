using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions;

public sealed class BooleanExtensionsTest
{
    [Fact]
    public void ThenWithAValueReturnsSomeWhenTrueAndNoneWhenFalse()
    {
        const int expectedValue = 1337;

        FunctionalAssert.Some(expectedValue, true.Then(expectedValue));
        FunctionalAssert.None(false.Then(expectedValue));
    }

    [Fact]
    public void ThenWithASelectorReturnsSomeWhenTrueAndNoneWhenFalse()
    {
        const string expectedValue = "value";

        FunctionalAssert.Some(expectedValue, true.Then(() => expectedValue));
        FunctionalAssert.None(false.Then(() => expectedValue));
    }

    [Fact]
    public void ThenWithAnOptionSelectorReturnsTheOptionWhenTrueAndNoneWhenFalse()
    {
        FunctionalAssert.Some(42, true.Then(() => Option.Some(42)));
        FunctionalAssert.None(true.Then(() => Option<int>.None));
        FunctionalAssert.None(false.Then(() => Option.Some(42)));
    }

    [Fact]
    public void TheSelectorIsOnlyCalledWhenTheConditionIsTrue()
    {
        FunctionalAssert.None(false.Then(FailOnCall.Function<string>));
        FunctionalAssert.None(false.Then(FailOnCall.Function<Option<string>>));
    }

    [Fact]
    public void ThenWorksOnAConditionExpression()
    {
        const int number = 42;

        FunctionalAssert.Some(number, (number % 2 == 0).Then(number));
        FunctionalAssert.None((number % 2 == 1).Then(number));
    }
}
