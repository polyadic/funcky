using System.Collections.Immutable;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class CartesianProductTest
    {
        [Fact]
        public void GivenTwoEmptySetsCartesianProductReturnsAnEmptySet()
        {
            var ints = Enumerable.Empty<int>();
            var strings = Enumerable.Empty<string>();

            Assert.Empty(ints.CartesianProduct(strings));
        }

        [Fact]
        public void GivenOneEmptySetCartesianProductReturnsAnEmptySet()
        {
            var ints = ImmutableList<int>.Empty.Add(1).Add(2).Add(42);
            var strings = Enumerable.Empty<string>();

            Assert.Empty(ints.CartesianProduct(strings));
        }

        [Fact]
        public void GivenTwoSingleElementSequencesWeGetASinglePairBack()
        {
            var ints = ImmutableList<int>.Empty.Add(1);
            var strings = ImmutableList<string>.Empty.Add("Hello");

            var product = ints.CartesianProduct(strings);
            Assert.Single(product);
        }

        [Fact]
        public void GivenTowSequencesWithMultipleItemsCartesianProductWithProjectReturnsAllPossibleCombinations()
        {
            var names = ImmutableList<string>.Empty.Add("Ruby").Add("Lucas");
            var greetings = ImmutableList<string>.Empty.Add("Hello").Add("Good Morning");

            var allPossibleGreetings = greetings.CartesianProduct(names, (greeting, name) => $"{greeting} {name}");
            Assert.Equal(new[] { "Hello Ruby", "Hello Lucas", "Good Morning Ruby", "Good Morning Lucas" }, allPossibleGreetings);
        }
    }
}
