using Xunit.Sdk;

namespace Funcky.Xunit.Test.FunctionalAssertClass;

public sealed class ErrorTest
{
    [Fact]
    public void DoesNotThrowForError()
    {
        FunctionalAssert.Error(Result<int>.Error(new ArgumentException("foo")));
    }

    [Fact]
    public void ThrowsForOk()
    {
        const string expectedMessage =
            """
            FunctionalAssert.Error() Failure: Values differ
            Expected: Error(...)
            Actual:   Ok(42)
            """;
        var exception = Assert.ThrowsAny<XunitException>(() => FunctionalAssert.Error(Result.Ok(42)));
        Assert.Equal(expectedMessage.ReplaceLineEndings(Environment.NewLine), exception.Message);
    }
}
