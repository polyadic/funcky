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
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

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

        private static HashSet<string> ToHashSet(IEnumerable<string> s)
            => s.ToHashSet();
    }
}
