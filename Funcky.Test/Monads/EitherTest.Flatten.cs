namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    [Fact]
    public void FlattenLeftIsLeft()
    {
        Assert.Equal("test", FunctionalAssert.Left(Either<string, Either<string, int>>.Left("test").Flatten()));
    }

    [Fact]
    public void FlattenRightLeftIsLeft()
    {
        Assert.Equal("test", FunctionalAssert.Left(Either<string, Either<string, int>>.Right(Either<string, int>.Left("test")).Flatten()));
    }

    [Fact]
    public void FlattenRightRightIsRight()
    {
        FunctionalAssert.Right(4711, Either<string, Either<string, int>>.Right(Either<string, int>.Right(4711)).Flatten());
    }
}
