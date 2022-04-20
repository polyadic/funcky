namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    public sealed class LeftOrNone
    {
        [Fact]
        public void ReturnsValueWhenEitherIsLeft()
        {
            FunctionalAssert.IsSome("test", Either<string, int>.Left("test").LeftOrNone());
        }

        [Fact]
        public void ReturnsNoneWhenEitherIsRight()
        {
            FunctionalAssert.IsNone(Either<string>.Return(10).LeftOrNone());
        }
    }

    public sealed class RightOrNone
    {
        [Fact]
        public void ReturnsValueWhenEitherIsRight()
        {
            FunctionalAssert.IsSome("test", Either<int>.Return("test").RightOrNone());
        }

        [Fact]
        public void ReturnsNoneWhenEitherIsLeft()
        {
            FunctionalAssert.IsNone(Either<int, string>.Left(10).RightOrNone());
        }
    }
}
