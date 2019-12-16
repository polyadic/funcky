using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public class PredicateCompositionTest
    {
        private const string DummyString = "foo";

        [Fact]
        public void AllReturnsTrueWhenNoPredicatesAreGiven()
        {
            var predicate = All<string>();
            Assert.True(predicate(DummyString));
        }

        [Fact]
        public void AllReturnsFalseWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(False, False, False);
            Assert.False(predicate(DummyString));
        }

        [Fact]
        public void AllReturnsFalseWhenOneOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(False, True, False);
            Assert.False(predicate(DummyString));
        }

        [Fact]
        public void AllReturnsTrueWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(True, True, True);
            Assert.True(predicate(DummyString));
        }

        [Fact]
        public void AnyReturnsTrueWhenNoPredicatesAreGiven()
        {
            var predicate = Any<string>();
            Assert.True(predicate(DummyString));
        }

        [Fact]
        public void AnyReturnsTrueWhenOneOfThePredicatesReturnsTrue()
        {
            var predicate = Any<string>(False, True, False);
            Assert.True(predicate(DummyString));
        }

        [Fact]
        public void AnyReturnsTrueWhenAllOfThePredicatesReturnsTrue()
        {
            var predicate = Any<string>(True, True, True);
            Assert.True(predicate(DummyString));
        }

        [Fact]
        public void AnyReturnsFalseWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = Any<string>(False, False, False);
            Assert.False(predicate(DummyString));
        }

        private static bool True<T>(T value) => true;

        private static bool False<T>(T value) => false;
    }
}
