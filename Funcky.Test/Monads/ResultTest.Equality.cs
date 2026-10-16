namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void OkResultsWithEqualValuesAreEqual()
    {
        var first = Result.Ok(42);
        var second = Result.Ok(42);

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void OkResultsWithDifferentValuesAreNotEqual()
    {
        var first = Result.Ok(42);
        var second = Result.Ok(43);

        Assert.False(first.Equals(second));
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void ErrorResultsWithTheSameExceptionAreEqual()
    {
        var exception = new InvalidOperationException("failure");
        var first = Result<int>.Error(exception);
        var second = Result<int>.Error(exception);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ErrorResultsWithDifferentExceptionsAreNotEqual()
    {
        var first = Result<int>.Error(new InvalidOperationException("failure"));
        var second = Result<int>.Error(new InvalidOperationException("failure"));

        Assert.False(first.Equals(second));
        Assert.True(first != second);
    }

    [Fact]
    public void AnOkResultIsNotEqualToAnErrorResult()
    {
        var ok = Result.Ok(42);
        var error = Result<int>.Error(new InvalidOperationException("failure"));

        Assert.False(ok.Equals(error));
        Assert.False(error.Equals(ok));
        Assert.True(ok != error);
    }

    [Fact]
    public void AResultIsNotEqualToAnObjectOfAnotherType()
    {
        var result = Result.Ok(42);

        Assert.False(result.Equals((object)42));
        Assert.False(result.Equals(null));
    }
}
