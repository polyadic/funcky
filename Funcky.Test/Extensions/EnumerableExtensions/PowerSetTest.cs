using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class PowerSetTest
    {
        [Fact]
        public void APowerSetIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.PowerSet();
        }

        [Fact]
        public void ThePowerSetOfTheEmptySetIsSetOfTheEmptySet()
        {
            var powerSet = Enumerable.Empty<string>().PowerSet();
            Assert.Empty(powerSet.First());
        }

        [Fact]
        public void ThePowerSetIsTheSetOfAllSubSets()
        {
            var set = new List<string> { "Alpha", "Beta", "Gamma" };
            var powerSet = set.PowerSet();

            Assert.Collection(
                powerSet,
                subset => { Assert.Equal(new string[] { }, subset); },
                subset => { Assert.Equal(new[] { "Alpha" }, subset); },
                subset => { Assert.Equal(new[] { "Beta" }, subset); },
                subset => { Assert.Equal(new[] { "Alpha", "Beta" }, subset); },
                subset => { Assert.Equal(new[] { "Gamma" }, subset); },
                subset => { Assert.Equal(new[] { "Alpha", "Gamma" }, subset); },
                subset => { Assert.Equal(new[] { "Beta", "Gamma" }, subset); },
                subset => { Assert.Equal(new[] { "Alpha", "Beta", "Gamma" }, subset); });
        }
    }
}
