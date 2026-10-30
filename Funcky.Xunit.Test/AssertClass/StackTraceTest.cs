using Xunit.Sdk;

namespace Funcky.Xunit.Test.AssertClass;

public sealed class StackTraceTest
{
    [Fact]
    public void StackTraceOfFailedAssertionDoesNotContainTheMonadOrLambdaFrames()
    {
        var stackTrace = CaptureStackTrace();
        Assert.DoesNotContain("Funcky.Monads.Option", stackTrace);
        Assert.DoesNotContain("<", stackTrace.Split('\n')[0]);
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
