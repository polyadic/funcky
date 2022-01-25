using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Monads
{
    public sealed partial class EitherTest
    {
        [Fact]
        public void CreateEitherLeftAndMatchCorrectly()
        {
            var value = Either<string, int>.Left("Error: not cool!");

            var hasLeft = value.Match(
                left: True,
                right: False);

            Assert.True(hasLeft);
        }

        [Fact]
        public void CreateEitherRightAndMatchCorrectly()
        {
            var value = Either<string, int>.Right(1337);

            var hasRight = value.Match(
                left: False,
                right: True);

            Assert.True(hasRight);
        }

        [Fact]
        public void MatchThrowsWhenEitherIsCreatedWithDefault()
        {
            var value = default(Either<string, int>);
            Assert.Throws<NotSupportedException>(() =>
                value.Match(left: Identity, right: i => i.ToString()));
        }

        [Theory]
        [MemberData(nameof(GetIntegerSums))]
        public void TheSumsOverEitherTypesShouldBeValid(Either<string, int> firstValue, Either<string, int> secondValue, Either<string, int> thirdValue, object reference)
        {
            var result =
                from first in firstValue
                from second in secondValue
                from third in thirdValue
                select first + second + third;

            var temp = result.Match(
                right: r => reference is int number && number == r,
                left: l => reference is string word && word == l);

            Assert.True(temp);
        }

        public static TheoryData<Either<string, int>, Either<string, int>, Either<string, int>, object> GetIntegerSums()
            => new()
            {
                { Either<string, int>.Right(5), Either<string, int>.Right(10), Either<string, int>.Right(15), 30 },
                { Either<string, int>.Right(1337), Either<string, int>.Right(42), Either<string, int>.Right(99), 1478 },
                { Either<string, int>.Right(45856), Either<string, int>.Right(58788), Either<string, int>.Right(699554), 804198 },
                { Either<string, int>.Right(5), Either<string, int>.Right(10), Either<string, int>.Left("Last"), "Last" },
                { Either<string, int>.Right(5), Either<string, int>.Left("Middle"), Either<string, int>.Left("Last"), "Middle" },
                { Either<string, int>.Left("First"), Either<string, int>.Left("Middle"), Either<string, int>.Left("Last"), "First" },
            };

        [Fact]
        public void NullableReferenceTypesAreSupported()
        {
            _ = Either<string?, int>.Left("foo");
            _ = Either<int, string?>.Right("foo");
            _ = Either<string?, int>.Left(null);
            _ = Either<int, string?>.Right(null);
        }

        [Fact]
        public void NullableValueTypesAreSupported()
        {
            _ = Either<int?, string>.Left(42);
            _ = Either<string, int?>.Right(42);
            _ = Either<int?, string>.Left(null);
            _ = Either<string, int?>.Right(null);
        }

        [Fact]
        public void MatchLeftOnEitherSupportsActions()
        {
            var value = Either<string, int>.Left("Error: not cool!");

            var hasLeft = false;
            var hasRight = false;

            value.Match(
                left: _ => Execute(() => hasLeft = true),
                right: _ => Execute(() => hasRight = true));

            Assert.True(hasLeft);
            Assert.False(hasRight);
        }

        [Fact]
        public void MatchRightOnEitherSupportsActions()
        {
            var value = Either<string, int>.Right(1337);

            var hasLeft = false;
            var hasRight = false;

            value.Match(
                left: _ => Execute(() => hasLeft = true),
                right: _ => Execute(() => hasRight = true));

            Assert.False(hasLeft);
            Assert.True(hasRight);
        }

        [Property]
        public Property FlippingAnEitherFlipsTheTypesOfTheEitherTheValueIsUntouched(int value)
        {
            var either = Either<string, int>.Right(value);
            var expected = Either<int, string>.Left(value);

            return (expected == either.Flip()).ToProperty();
        }

        private static void Execute(Action action) => action();
    }
}
