using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class LeftTest
{
    [Fact]
    public void DoesNotThrowForMatchingLeft()
    {
        FunctionalAssert.Left(10, Either<int, string>.Left(10));
    }

    [Fact]
    public void DoesNotThrowForLeft()
    {
        FunctionalAssert.Left(Either<int, string>.Left(10));
    }

    [Fact]
    public void ThrowsForMismatchingLeft()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Left() Failure: Values differ
            Expected: Left(10)
            Actual:   Left(11)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Left(10, Either<int, string>.Left(11)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForRightWithExpectedValue()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Left() Failure: Values differ
            Expected: Left(10)
            Actual:   Right("right")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Left(10, Either<int, string>.Right("right")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForRight()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Left() Failure: Values differ
            Expected: Left(...)
            Actual:   Right("right")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Left(Either<int, string>.Right("right")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
