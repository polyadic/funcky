namespace Funcky.Test.FunctionalClass;

public sealed class NoOperationAsyncTest
{
    [Fact]
    public async Task GivenTheNoOperationAsyncFunctionWeCanApplyItToMatch()
    {
        var none = Option<int>.None;

        var sideEffect = 0;
        await none.Match(
            none: NoOperationAsync,
            some: async i => await Task.Run(() => sideEffect = i));

        Assert.Equal(0, sideEffect);
    }
}
