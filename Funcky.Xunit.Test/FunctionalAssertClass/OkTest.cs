using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class OkTest
{
    [Fact]
    public void DoesNotThrowForMatchingOk()
    {
        FunctionalAssert.Ok(10, Result.Ok(10));
    }

    [Fact]
    public void DoesNotThrowForOk()
    {
        FunctionalAssert.Ok(Result.Ok(10));
    }

    [Fact]
    public void ThrowsForMismatchingOk()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Ok() Failure: Values differ
            Expected: Ok(10)
            Actual:   Ok(11)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Ok(10, Result.Ok(11)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForErrorWithExpectedValue()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Ok() Failure: Values differ
            Expected: Ok(10)
            Actual:   Error(System.ArgumentException: "message")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Ok(10, Result<int>.Error(new ArgumentException("message"))));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForError()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Ok() Failure: Values differ
            Expected: Ok(...)
            Actual:   Error(System.ArgumentException: "message")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Ok(Result<int>.Error(new ArgumentException("message"))));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
