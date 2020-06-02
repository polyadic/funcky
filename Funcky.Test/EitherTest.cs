using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class EitherTest
    {
        [Fact]
        public void CreateEitherLeftAndMatchCorrectly()
        {
            var value = Either<string, int>.Left("Error: not cool!");

            var hasLeft = value.Match(
                left: l => true,
                right: r => false);

            Assert.True(hasLeft);
        }

        [Fact]
        public void CreateEitherRightAndMatchCorrectly()
        {
            var value = Either<string, int>.Right(1337);

            var hasRight = value.Match(
                left: l => false,
                right: r => true);

            Assert.True(hasRight);
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
            => new TheoryData<Either<string, int>, Either<string, int>, Either<string, int>, object>
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
    }
}
