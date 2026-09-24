using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class RightTest
{
    [Fact]
    public void DoesNotThrowForMatchingRight()
    {
        FunctionalAssert.Right(10, Either<string, int>.Right(10));
    }

    [Fact]
    public void DoesNotThrowForRight()
    {
        FunctionalAssert.Right(Either<string, int>.Right(10));
    }

    [Fact]
    public void ThrowsForMismatchingRight()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Right() Failure: Values differ
            Expected: Right(10)
            Actual:   Right(11)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Right(10, Either<string, int>.Right(11)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForLeftWithExpectedValue()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Right() Failure: Values differ
            Expected: Right(10)
            Actual:   Left("left")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Right(10, Either<string, int>.Left("left")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForLeft()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Right() Failure: Values differ
            Expected: Right(...)
            Actual:   Left("left")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Right(Either<string, int>.Left("left")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
