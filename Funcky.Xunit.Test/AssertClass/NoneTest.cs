using Xunit.Sdk;

namespace Funcky.Xunit.Test.AssertClass;

public sealed class NoneTest
{
    [Fact]
    public void DoesNotThrowForNone()
    {
        Assert.None(Option<string>.None);
    }

    [Fact]
    public void ThrowsForSome()
    {
        const string expectedMessage =
            """
            Assert.None() Failure: Values differ
            Expected: None
            Actual:   Some("something")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.None(Option.Some("something")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
