using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public class PredicateCompositionTest
    {
        private const string PlaceholderValue = "foo";

        [Fact]
        public void AllReturnsTrueWhenNoPredicatesAreGiven()
        {
            var predicate = All<string>();
            Assert.True(predicate(PlaceholderValue));
        }

        [Fact]
        public void AllReturnsFalseWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(False, False, False);
            Assert.False(predicate(PlaceholderValue));
        }

        [Fact]
        public void AllReturnsFalseWhenOneOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(False, True, False);
            Assert.False(predicate(PlaceholderValue));
        }

        [Fact]
        public void AllReturnsTrueWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = All<string>(True, True, True);
            Assert.True(predicate(PlaceholderValue));
        }

        [Fact]
        public void AnyReturnsFalseWhenNoPredicatesAreGiven()
        {
            var predicate = Any<string>();
            Assert.False(predicate(PlaceholderValue));
        }

        [Fact]
        public void AnyReturnsTrueWhenOneOfThePredicatesReturnsTrue()
        {
            var predicate = Any<string>(False, True, False);
            Assert.True(predicate(PlaceholderValue));
        }

        [Fact]
        public void AnyReturnsTrueWhenAllOfThePredicatesReturnsTrue()
        {
            var predicate = Any<string>(True, True, True);
            Assert.True(predicate(PlaceholderValue));
        }

        [Fact]
        public void AnyReturnsFalseWhenAllOfThePredicatesReturnsFalse()
        {
            var predicate = Any<string>(False, False, False);
            Assert.False(predicate(PlaceholderValue));
        }

        [Fact]
        public void NotReturnsPredicateThatReturnsTrueWhenOriginalPredicateReturnsFalse()
        {
            var negated = Not<string>(False);
            Assert.False(negated(PlaceholderValue));
        }

        [Fact]
        public void NotReturnsPredicateThatReturnsFalseWhenOriginalPredicateReturnsTrue()
        {
            var negated = Not<string>(True);
            Assert.False(negated(PlaceholderValue));
        }
    }
}
