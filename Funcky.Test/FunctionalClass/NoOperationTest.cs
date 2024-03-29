namespace Funcky.Test.FunctionalClass;

public sealed class NoOperationTest
{
    [Fact]
    public void GivenTheNoOperationFunctionWeCanApplyItToMatch()
    {
        var none = Option<int>.None;

        var sideEffect = 0;
        none.Switch(none: NoOperation, some: i => sideEffect = i);

        Assert.Equal(0, sideEffect);
    }
}
