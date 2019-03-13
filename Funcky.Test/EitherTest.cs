using System.Collections;
using System.Collections.Generic;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class EitherTest
    {
        public class IntegerSums : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { Either<string, int>.Right(5), Either<string, int>.Right(10), Either<string, int>.Right(15), 30 };
                yield return new object[] { Either<string, int>.Right(1337), Either<string, int>.Right(42), Either<string, int>.Right(99), 1478 };
                yield return new object[] { Either<string, int>.Right(45856), Either<string, int>.Right(58788), Either<string, int>.Right(699554), 804198 };
                yield return new object[] { Either<string, int>.Right(5), Either<string, int>.Right(10), Either<string, int>.Left("Last"), "Last" };
                yield return new object[] { Either<string, int>.Right(5), Either<string, int>.Left("Middle"), Either<string, int>.Left("Last"), "Middle" };
                yield return new object[] { Either<string, int>.Left("First"), Either<string, int>.Left("Middle"), Either<string, int>.Left("Last"), "First" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

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
        [ClassData(typeof(IntegerSums))]
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


    }
}
