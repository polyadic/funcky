namespace Funcky.Test.Extensions;

public sealed class ExceptionExtensionsTest
{
    [Fact]
    public void InnerExceptionOrNoneReturnsTheInnerException()
    {
        var inner = new InvalidOperationException("inner");

        FunctionalAssert.Some(inner, new Exception("outer", inner).InnerExceptionOrNone);
    }

    [Fact]
    public void InnerExceptionOrNoneReturnsNoneWithoutAnInnerException()
    {
        FunctionalAssert.None(new Exception("outer").InnerExceptionOrNone);
    }
}
