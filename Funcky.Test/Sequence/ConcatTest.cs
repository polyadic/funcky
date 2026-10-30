using System.Collections.Immutable;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test;

public sealed class ConcatTest
{
    [Fact]
    public void ConcatenatedSequenceIsEmptyWhenNoSourcesAreProvided()
    {
        Assert.Empty(Enumerable.Concat<object>());
    }

    [Fact]
    public void ConcatenatedSequenceIsEmptyWhenAllSourcesAreEmpty()
    {
        Assert.Empty(Enumerable.Concat(ImmutableArray.Create(Enumerable.Empty<object>(), Enumerable.Empty<object>(), Enumerable.Empty<object>())));
    }

    [Property]
    public Property ConcatenatedSequenceContainsElementsFromAllSourcesInOrder(int[][] sources)
    {
        var expected = sources.Aggregate(ImmutableArray<int>.Empty, (l, s) => l.AddRange(s));
        var actual = Enumerable.Concat(sources);
        return expected.SequenceEqual(actual).ToProperty();
    }
}
