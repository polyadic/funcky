using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class NoneTest
{
    [Fact]
    public void DoesNotThrowForNone()
    {
        FunctionalAssert.None(Option<string>.None);
    }

    [Fact]
    public void ThrowsForSome()
    {
        const string expectedMessage =
            """
            FunctionalAssert.None() Failure: Values differ
            Expected: None
            Actual:   Some("something")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.None(Option.Some("something")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
