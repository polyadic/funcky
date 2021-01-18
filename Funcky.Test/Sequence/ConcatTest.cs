using System.Collections.Immutable;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Funcky.Test
{
    public sealed class ConcatTest
    {
        [Fact]
        public void ConcatenatedSequenceIsEmptyWhenNoSourcesAreProvided()
        {
            Assert.Empty(Sequence.Concat<object>());
        }

        [Fact]
        public void ConcatenatedSequenceIsEmptyWhenAllSourcesAreEmpty()
        {
            Assert.Empty(Sequence.Concat(ImmutableArray.Create(Enumerable.Empty<object>(), Enumerable.Empty<object>(), Enumerable.Empty<object>())));
        }

        [Property]
        public Property ConcatenatedSequenceContainsElementsFromAllSourcesInOrder(int[][] sources)
        {
            var expected = sources.Aggregate(ImmutableArray<int>.Empty, (l, s) => l.AddRange(s));
            var actual = Sequence.Concat(sources);
            return expected.SequenceEqual(actual).ToProperty();
        }
    }
}
