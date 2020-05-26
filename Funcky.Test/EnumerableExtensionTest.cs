using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class EnumerableExtensionTest
    {
        [Fact]
        public void GivenAnObjectWeCreateAnIEnumerableWithYield()
        {
            AcceptIntegers(42.Yield());

            Unit unit;
            AcceptUnits(unit.Yield());
        }

        [Fact]
        public void GivenAnEnumerableAndInjectWeCanApplySideffectsToEnumberables()
        {
            int sideEffect = 0;
            var numbers = new List<int> { 1, 2, 3, 42 };

            var numbersWithSideEffect = numbers
                .Inspect(n => { ++sideEffect; });

            Assert.Equal(0, sideEffect);

            numbersWithSideEffect.Each(n => { });

            Assert.Equal(numbers.Count, sideEffect);
        }

        [Fact]
        public void WhereSelectFiltersEmptyValues()
        {
            var input = Enumerable.Range(0, 10);
            var expectedResult = new[] { 0, 4, 16, 36, 64 };
            var result = input.WhereSelect(SquareEvenNumbers);
            Assert.Equal(expectedResult, result);
        }

        private static Option<int> SquareEvenNumbers(int number)
            => IsEven(number) ? Option.Some(number * number) : Option<int>.None();

        private static bool IsEven(int number) => number % 2 == 0;

        private void AcceptIntegers(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Assert.Equal(42, value);
            }
        }

        private void AcceptUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                Assert.Equal(default, unit);
            }
        }
    }
}
