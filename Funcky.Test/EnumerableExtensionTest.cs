using System.Collections.Generic;
using Funcky.Extensions;
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
                Assert.Equal(new Unit(), unit);
            }
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

    }
}
