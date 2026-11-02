using Xunit.Sdk;

namespace Funcky.Xunit.Test.Extensions;

public sealed class AssertExtensionsTest
{
    [Fact]
    public void SomeDoesNotThrowForMatchingSome()
    {
        Assert.Some(10, Option.Some(10));
        var value = Assert.Some(Option.Some(10));
        Assert.Equal(10, value);
    }

    [Fact]
    public void SomeThrowsForNone()
    {
        const string expectedMessage =
            """
            Assert.Some() Failure: Values differ
            Expected: Some(10)
            Actual:   None
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Some(10, Option<int>.None));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Some(Option<int>.None));
    }

    [Fact]
    public void NoneDoesNotThrowForNone()
    {
        Assert.None(Option<int>.None);
    }

    [Fact]
    public void NoneThrowsForSome()
    {
        const string expectedMessage =
            """
            Assert.None() Failure: Values differ
            Expected: None
            Actual:   Some(10)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.None(Option.Some(10)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }

    [Fact]
    public void OkDoesNotThrowForMatchingOk()
    {
        Assert.Ok(10, Result.Ok(10));
        var value = Assert.Ok(Result.Ok(10));
        Assert.Equal(10, value);
    }

    [Fact]
    public void OkThrowsForError()
    {
        const string expectedMessage =
            """
            Assert.Ok() Failure: Values differ
            Expected: Ok(10)
            Actual:   Error(System.InvalidOperationException: "boom")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Ok(10, Result<int>.Error(new InvalidOperationException("boom"))));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Ok(Result<int>.Error(new InvalidOperationException("boom"))));
    }

    [Fact]
    public void ErrorReturnsTheErrorForError()
    {
        var error = new InvalidOperationException("boom");
        Assert.Same(error, Assert.Error(Result<int>.Error(error)));
        Assert.Same(error, Assert.Error<InvalidOperationException, int>(Result<int>.Error(error)));
    }

    [Fact]
    public void ErrorThrowsForOk()
    {
        const string expectedMessage =
            """
            Assert.Error() Failure: Values differ
            Expected: Error(...)
            Actual:   Ok(10)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Error(Result.Ok(10)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Error<InvalidOperationException, int>(Result.Ok(10)));
    }

    [Fact]
    public void ErrorThrowsForErrorOfAnotherType()
    {
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Error<ArgumentException, int>(Result<int>.Error(new InvalidOperationException("boom"))));
    }

    [Fact]
    public void LeftDoesNotThrowForMatchingLeft()
    {
        Assert.Left("failure", Either<string, int>.Left("failure"));
        var left = Assert.Left(Either<string, int>.Left("failure"));
        Assert.Equal("failure", left);
    }

    [Fact]
    public void LeftThrowsForRight()
    {
        const string expectedMessage =
            """
            Assert.Left() Failure: Values differ
            Expected: Left("failure")
            Actual:   Right(42)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Left("failure", Either<string>.Return(42)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Left(Either<string>.Return(42)));
    }

    [Fact]
    public void RightDoesNotThrowForMatchingRight()
    {
        Assert.Right(42, Either<string>.Return(42));
        var right = Assert.Right(Either<string>.Return(42));
        Assert.Equal(42, right);
    }

    [Fact]
    public void RightThrowsForLeft()
    {
        const string expectedMessage =
            """
            Assert.Right() Failure: Values differ
            Expected: Right(42)
            Actual:   Left("failure")
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => Assert.Right(42, Either<string, int>.Left("failure")));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
        _ = Assert.ThrowsAny<XunitException>(() => Assert.Right(Either<string, int>.Left("failure")));
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void StackTraceOfFailedAssertionStartsAtTheTestMethod()
    {
        var firstLine = CaptureStackTrace().Split('\n')[0];
        Assert.DoesNotContain("Funcky.Extensions.AssertExtensions.", firstLine);
        Assert.Contains(nameof(CaptureStackTrace), firstLine);
    }
#else
    // Frameworks without [StackTraceHidden] cannot hide the assertion frame; it is the only frame above the test code.
    [Fact]
    public void StackTraceOfFailedAssertionStartsAtTheAssertionMethod()
    {
        var lines = CaptureStackTrace().Split('\n');
        Assert.Contains("Funcky.Extensions.AssertExtensions.Some", lines[0]);
        Assert.Contains(nameof(CaptureStackTrace), lines[1]);
    }
#endif

    private static string CaptureStackTrace()
    {
        try
        {
            Assert.Some(Option<int>.None);
        }
        catch (XunitException exception)
        {
            return exception.StackTrace ?? string.Empty;
        }

        throw new InvalidOperationException("Assert.Some did not throw");
    }
}
