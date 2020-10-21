using System.Collections.Generic;
using Funcky.Extensions;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class InspectTest
    {
        [Fact]
        public void GivenAnEnumerableAndInjectWeCanApplySideEffectsToEnumerables()
        {
            var sideEffect = 0;
            var numbers = new List<int> { 1, 2, 3, 42 };

            var numbersWithSideEffect = numbers
                .Inspect(n => { ++sideEffect; });

            Assert.Equal(0, sideEffect);

            numbersWithSideEffect.ForEach(NoOperation);

            Assert.Equal(numbers.Count, sideEffect);
        }
    }
}
