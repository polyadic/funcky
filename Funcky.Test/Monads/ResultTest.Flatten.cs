namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void FlattenErrorIsError()
    {
        FunctionalAssert.Error(Result<Result<int>>.Error(new Exception()).Flatten());
    }

    [Fact]
    public void FlattenSomeNoneIsNone()
    {
        FunctionalAssert.Error(Result.Ok(Result<int>.Error(new Exception())).Flatten());
    }

    [Fact]
    public void FlattenSomeSomeIsSome()
    {
        Assert.Equal(4711, FunctionalAssert.Ok(Result.Ok(Result.Ok(4711)).Flatten()));
    }
}
