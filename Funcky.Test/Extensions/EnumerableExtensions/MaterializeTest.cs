using System.Collections;
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

        private static HashSet<string> ToHashSet(IEnumerable<string> s)
            => s.ToHashSet();

        private record FailOnEnumerateCollection<T>(int Count) : ICollection<T>
        {
            public bool IsReadOnly => true;

            public IEnumerator<T> GetEnumerator() => throw new XunitException("Should not be enumerated");

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(T item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            public bool Contains(T item) => throw new NotSupportedException();

            public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();

            public bool Remove(T item) => throw new NotSupportedException();
        }

        private sealed record FailOnEnumerateList<T>(int Count) : FailOnEnumerateCollection<T>(Count), IList<T>
        {
            public T this[int index]
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public int IndexOf(T item) => throw new NotSupportedException();

            public void Insert(int index, T item) => throw new NotSupportedException();

            public void RemoveAt(int index) => throw new NotSupportedException();
        }
    }
}
