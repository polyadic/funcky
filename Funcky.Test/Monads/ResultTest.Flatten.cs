namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void FlattenErrorIsError()
    {
        Assert.Error(Result<Result<int>>.Error(new Exception()).Flatten());
    }

    [Fact]
    public void FlattenOkErrorIsError()
    {
        Assert.Error(Result.Ok(Result<int>.Error(new Exception())).Flatten());
    }

    [Fact]
    public void FlattenOkOkIsOk()
    {
        Assert.Ok(4711, Result.Ok(Result.Ok(4711)).Flatten());
    }
}
