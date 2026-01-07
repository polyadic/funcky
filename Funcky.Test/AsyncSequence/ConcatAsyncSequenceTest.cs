using System.Collections.Immutable;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class ConcatAsyncSequenceTest
{
    [Fact]
    public async Task ConcatenatedSequenceIsEmptyWhenNoSourcesAreProvidedAsync()
    {
        await AsyncAssert.Empty(AsyncSequence.Concat<object>());
    }

    [Fact]
    public async Task ConcatenatedSequenceIsEmptyWhenAllSourcesAreEmptyAsync()
    {
        await AsyncAssert.Empty(AsyncSequence.Concat(Enumerable.Empty<object>().ToAsyncEnumerable(), Enumerable.Empty<object>().ToAsyncEnumerable(), Enumerable.Empty<object>().ToAsyncEnumerable()));
    }

    [Property]
    public Property ConcatenatedSequenceContainsElementsFromAllSourcesInOrder(int[][] sources)
    {
        var expected = sources.Aggregate(ImmutableArray<int>.Empty, (l, s) => l.AddRange(s)).ToAsyncEnumerable();

        var innerOuterAsync = sources.Select(source => source.ToAsyncEnumerable()).ToAsyncEnumerable();
        var innerAsync = sources.Select(source => source.ToAsyncEnumerable());
        IAsyncEnumerable<IEnumerable<int>> outerAsync = sources.ToAsyncEnumerable();

        var result = expected.SequenceEqualAsync(AsyncSequence.Concat(innerOuterAsync)).Result
                     && expected.SequenceEqualAsync(AsyncSequence.Concat(innerAsync)).Result
                     && expected.SequenceEqualAsync(AsyncSequence.Concat(outerAsync)).Result;

        return result.ToProperty();
    }
}
