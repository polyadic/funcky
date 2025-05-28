namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Fact]
    public void FlattenNoneIsNone()
    {
        FunctionalAssert.None(Option<Option<int>>.None.Flatten());
    }

    [Fact]
    public void FlattenSomeNoneIsNone()
    {
        FunctionalAssert.None(Option.Some(Option<int>.None).Flatten());
    }

    [Fact]
    public void FlattenSomeSomeIsSome()
    {
        Assert.Equal(4711, FunctionalAssert.Some(Option.Some(Option.Some(4711)).Flatten()));
    }
}
