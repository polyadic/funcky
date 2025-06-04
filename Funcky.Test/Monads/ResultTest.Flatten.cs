namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void FlattenErrorIsError()
    {
        FunctionalAssert.Error(Result<Result<int>>.Error(new Exception()).Flatten());
    }

    [Fact]
    public void FlattenOkErrorIsError()
    {
        FunctionalAssert.Error(Result.Ok(Result<int>.Error(new Exception())).Flatten());
    }

    [Fact]
    public void FlattenSomeSomeIsSome()
    {
        FunctionalAssert.Ok(4711, Result.Ok(Result.Ok(4711)).Flatten());
    }
}
