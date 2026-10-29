namespace Funcky.Test.Extensions;

public sealed class ExceptionExtensionsTest
{
    [Fact]
    public void InnerExceptionOrNoneReturnsTheInnerException()
    {
        var inner = new InvalidOperationException("inner");

        Assert.Some(inner, new Exception("outer", inner).InnerExceptionOrNone);
    }

    [Fact]
    public void InnerExceptionOrNoneReturnsNoneWithoutAnInnerException()
    {
        Assert.None(new Exception("outer").InnerExceptionOrNone);
    }
}
