using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class InspectTest
    {
        [Fact]
        public void InspectIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.Inspect(NoOperation);
        }

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
