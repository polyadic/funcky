using System.Collections.Immutable;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class CartesianProductTest
    {
        [Fact]
        public void CartesianProductIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.CartesianProduct(doNotEnumerate);
        }

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
            var ints = ImmutableList.Create(1, 2, 3);
            var strings = Enumerable.Empty<string>();

            Assert.Empty(ints.CartesianProduct(strings));
        }

        [Fact]
        public void GivenTwoSingleElementSequencesWeGetASinglePairBack()
        {
            var ints = ImmutableList.Create(1);
            var strings = ImmutableList.Create("Hello");

            var product = ints.CartesianProduct(strings);
            Assert.Single(product);
        }

        [Fact]
        public void GivenTwoSequencesWithMultipleItemsCartesianProductWithProjectReturnsAllPossibleCombinations()
        {
            var names = ImmutableList.Create("Ruby", "Lucas");
            var greetings = ImmutableList.Create("Hello", "Good Morning");

            var allPossibleGreetings = greetings.CartesianProduct(names, (greeting, name) => $"{greeting} {name}");
            Assert.Equal(new[] { "Hello Ruby", "Hello Lucas", "Good Morning Ruby", "Good Morning Lucas" }, allPossibleGreetings);
        }

        [Fact]
        public void GivenTwoDifferentlySizedSequencesTheCartesianProductReturnsAllPossibleCombinations()
        {
            var suits = ImmutableList.Create("♠", "♣", "♥", "♦");
            var values = ImmutableList.Create("2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A");

            var cards = suits.CartesianProduct(values, (suit, value) => $"{value}{suit}").ToList();

            Assert.Equal(52, cards.Count);
            Assert.Equal("2♠", cards.First());
            Assert.Equal("A♦", cards.Last());
        }
    }
}
