#if !SET_CURRENT_STACK_TRACE_SUPPORTED
namespace Funcky.Test.Monads;

// These tests run only on frameworks without ExceptionDispatchInfo.SetCurrentStackTrace, where Funcky
// captures the stack trace through reflection on private runtime members.
public sealed partial class ResultTest
{
    [Fact]
    public void ErrorCapturesTheStackTraceOfAnUnthrownExceptionThroughTheShim()
    {
        var exception = new InvalidOperationException("never thrown");
        Assert.Null(exception.StackTrace);

        _ = Result<int>.Error(exception);

        Assert.NotNull(exception.StackTrace);
        Assert.Contains(nameof(ErrorCapturesTheStackTraceOfAnUnthrownExceptionThroughTheShim), exception.StackTrace);
    }

    [Fact]
    public void ErrorLeavesAnExistingStackTraceAlone()
    {
        var exception = Assert.Throws<InvalidOperationException>(ThrowInvalidOperation);
        var stackTrace = exception.StackTrace;

        _ = Result<int>.Error(exception);

        Assert.Equal(stackTrace, exception.StackTrace);
    }

    private static void ThrowInvalidOperation()
        => throw new InvalidOperationException("thrown");
}
#endif
