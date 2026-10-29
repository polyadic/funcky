using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions;

public sealed class BooleanExtensionsTest
{
    [Fact]
    public void ThenWithAValueReturnsSomeWhenTrueAndNoneWhenFalse()
    {
        const int expectedValue = 1337;

        Assert.Some(expectedValue, true.Then(expectedValue));
        Assert.None(false.Then(expectedValue));
    }

    [Fact]
    public void ThenWithASelectorReturnsSomeWhenTrueAndNoneWhenFalse()
    {
        const string expectedValue = "value";

        Assert.Some(expectedValue, true.Then(() => expectedValue));
        Assert.None(false.Then(() => expectedValue));
    }

    [Fact]
    public void ThenWithAnOptionSelectorReturnsTheOptionWhenTrueAndNoneWhenFalse()
    {
        Assert.Some(42, true.Then(() => Option.Some(42)));
        Assert.None(true.Then(() => Option<int>.None));
        Assert.None(false.Then(() => Option.Some(42)));
    }

    [Fact]
    public void TheSelectorIsOnlyCalledWhenTheConditionIsTrue()
    {
        Assert.None(false.Then(FailOnCall.Function<string>));
        Assert.None(false.Then(FailOnCall.Function<Option<string>>));
    }

    [Fact]
    public void ThenWorksOnAConditionExpression()
    {
        const int number = 42;

        Assert.Some(number, (number % 2 == 0).Then(number));
        Assert.None((number % 2 == 1).Then(number));
    }
}
