using Xunit.Sdk;

namespace Funcky.Xunit.Test.AssertClass;

public sealed class ErrorTest
{
    [Fact]
    public void DoesNotThrowForError()
    {
        Assert.Error(Result<int>.Error(new ArgumentException("foo")));
    }

    [Fact]
    public void ThrowsForOk()
    {
        const string expectedMessage =
            """
            Assert.Error() Failure: Values differ
            Expected: Error(...)
            Actual:   Ok(42)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Error(Result.Ok(42)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ReturnsTheErrorTypedWhenItIsOfTheExpectedType()
    {
        var exception = new ArgumentException("foo");

        var error = Assert.Error<ArgumentException, int>(Result<int>.Error(exception));

        Assert.Same(exception, error);
    }

    [Fact]
    public void ThrowsForAnErrorOfAnotherType()
    {
        const string expectedMessage =
            """
            Assert.Error() Failure: Values differ
            Expected: Error(ArgumentException)
            Actual:   Error(InvalidOperationException)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Error<ArgumentException, int>(Result<int>.Error(new InvalidOperationException("foo"))));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void ThrowsForOkWhenAnExceptionTypeIsExpected()
    {
        const string expectedMessage =
            """
            Assert.Error() Failure: Values differ
            Expected: Error(ArgumentException)
            Actual:   Ok(42)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Error<ArgumentException, int>(Result.Ok(42)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
