using System.Collections.Immutable;
using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class MaterializeTest
    {
        [Fact]
        public void MaterializeEnumeratesNonCollection()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            Assert.Throws<XunitException>(() => doNotEnumerate.Materialize());
        }

        [Fact]
        public void MaterializeDoesNotEnumerateCollectionTypes()
        {
            var list = new List<string> { "something" };

            Assert.IsType<List<string>>(list.Materialize());
            Assert.IsType<List<string>>(list.Materialize(ToHashSet));
        }

        [Fact]
        public void MaterializeReturnsImmutableCollectionWhenEnumerate()
        {
            var sequence = Enumerable.Repeat("Hello world!", 3);

            Assert.IsType<ImmutableList<string>>(sequence.Materialize());
        }

        [Fact]
        public void MaterializeWithMaterializationReturnsCorrectCollectionWhenEnumerate()
        {
            var list = Enumerable.Repeat("Hello world!", 3);

            Assert.IsType<HashSet<string>>(list.Materialize(ToHashSet));
        }

        [Fact]
        public void MaterializeDoesNotEnumerateCollectionWhichImplementsICollectionOnly()
        {
            var list = new FailOnEnumerateCollection<int>(Count: 10);
            _ = list.Materialize();
        }

        [Fact]
        public void MaterializedICollectionHasCorrectCount()
        {
            const int count = 10;
            var list = new FailOnEnumerateCollection<int>(Count: count);
            Assert.Equal(count, list.Materialize().Count);
        }

        [Fact]
        public void MaterializingAnICollectionReturnsAnICollection()
        {
            var collection = new FailOnEnumerateCollection<int>(Count: 0);
            Assert.IsAssignableFrom<ICollection<int>>(collection.Materialize());
        }

        [Fact]
        public void MaterializingAnIListReturnsAnIList()
        {
            var collection = new FailOnEnumerateList<int>(Count: 0);
            Assert.IsAssignableFrom<IList<int>>(collection.Materialize());
        }

        [Fact]
        public void MaterializingAnIListReturnsAnIReadOnlyList()
        {
            var collection = new FailOnEnumerateList<int>(Count: 0);
            Assert.IsAssignableFrom<IReadOnlyList<int>>(collection.Materialize());
        }

        private static HashSet<string> ToHashSet(IEnumerable<string> s)
            => s.ToHashSet();
    }
}
