using Xunit.Sdk;

namespace Funcky.Xunit.Test.AssertClass;

public sealed class RightTest
{
    [Fact]
    public void DoesNotThrowForMatchingRight()
    {
        Assert.Right(10, Either<string, int>.Right(10));
    }

    [Fact]
    public void DoesNotThrowForRight()
    {
        Assert.Right(Either<string, int>.Right(10));
    }

    [Fact]
    public void ThrowsForMismatchingRight()
    {
        const string expectedMessage =
            """
            Assert.Right() Failure: Values differ
            Expected: Right(10)
            Actual:   Right(11)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Right(10, Either<string, int>.Right(11)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForLeftWithExpectedValue()
    {
        const string expectedMessage =
            """
            Assert.Right() Failure: Values differ
            Expected: Right(10)
            Actual:   Left("left")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Right(10, Either<string, int>.Left("left")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForLeft()
    {
        const string expectedMessage =
            """
            Assert.Right() Failure: Values differ
            Expected: Right(...)
            Actual:   Left("left")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Right(Either<string, int>.Left("left")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
