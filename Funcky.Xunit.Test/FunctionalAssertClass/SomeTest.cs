using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class SomeTest
{
    [Fact]
    public void DoesNotThrowForMatchingSome()
    {
        FunctionalAssert.Some(10, Option.Some(10));
    }

    [Fact]
    public void DoesNotThrowForRight()
    {
        _ = FunctionalAssert.Some(Option.Some(10));
    }

    [Fact]
    public void ThrowsForMismatchingSome()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Some() Failure: Values differ
            Expected: Some(10)
            Actual:   Some(11)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Some(10, Option.Some(11)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForNoneWithExpectedValue()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Some() Failure: Values differ
            Expected: Some(10)
            Actual:   None
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Some(10, Option<int>.None));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForNone()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Some() Failure: Values differ
            Expected: Some(...)
            Actual:   None
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Some(Option<int>.None));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
