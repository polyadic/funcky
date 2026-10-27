namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    public sealed class LeftOrNone
    {
        [Fact]
        public void ReturnsValueWhenEitherIsLeft()
        {
            Assert.Some("test", Either<string, int>.Left("test").LeftOrNone());
        }

        [Fact]
        public void ReturnsNoneWhenEitherIsRight()
        {
            Assert.None(Either<string>.Return(10).LeftOrNone());
        }
    }

    public sealed class RightOrNone
    {
        [Fact]
        public void ReturnsValueWhenEitherIsRight()
        {
            Assert.Some("test", Either<int>.Return("test").RightOrNone());
        }

        [Fact]
        public void ReturnsNoneWhenEitherIsLeft()
        {
            Assert.None(Either<int, string>.Left(10).RightOrNone());
        }
    }
}
