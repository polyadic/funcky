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
        FunctionalAssert.Some(4711, Option.Some(Option.Some(4711)).Flatten());
    }
}
