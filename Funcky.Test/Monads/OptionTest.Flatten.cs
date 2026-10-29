namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Fact]
    public void FlattenNoneIsNone()
    {
        Assert.None(Option<Option<int>>.None.Flatten());
    }

    [Fact]
    public void FlattenSomeNoneIsNone()
    {
        Assert.None(Option.Some(Option<int>.None).Flatten());
    }

    [Fact]
    public void FlattenSomeSomeIsSome()
    {
        Assert.Some(4711, Option.Some(Option.Some(4711)).Flatten());
    }
}
