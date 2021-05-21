using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class InspectTest
    {
        [Fact]
        public void InspectIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            _ = doNotEnumerate.Inspect(NoOperation);
        }

        [Fact]
        public async Task GivenAnEnumerableAndInjectWeCanApplySideEffectsToEnumerables()
        {
            var sideEffect = 0;
            var numbers = new List<int> { 1, 2, 3, 42 }.ToAsyncEnumerable();

            var numbersWithSideEffect = numbers
                .Inspect(n => { ++sideEffect; });

            Assert.Equal(0, sideEffect);

            await numbersWithSideEffect.ForEachAsync(NoOperation<int>);

            Assert.Equal(await numbers.CountAsync(), sideEffect);
        }
    }
}
