// ReSharper disable PossibleMultipleEnumeration

using System.Collections.Immutable;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;
using Funcky.Test.Internal;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MergeTest
{
    [Fact]
    public void MergeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.Merge(doNotEnumerate);
    }

    [Fact]
    public Task MergeEmptySequencesResultsInAnEmptySequence()
    {
        var emptySequence = AsyncEnumerable.Empty<int>();

        return AsyncAssert.Empty(emptySequence.Merge(emptySequence, emptySequence, emptySequence));
    }

    [Fact]
    public async Task MergeAnEmptySequenceWithANonEmptySequenceResultsInTheNonEmptySequenceAsync()
    {
        var nonEmptySequence = AsyncSequence.Return(1, 2, 4, 7);
        var emptySequence = AsyncEnumerable.Empty<int>();

        await AsyncAssert.Equal(nonEmptySequence, nonEmptySequence.Merge(emptySequence));
        await AsyncAssert.Equal(nonEmptySequence, emptySequence.Merge(nonEmptySequence));
    }

    [Property]
    public void TwoSingleSequencesAreMergedCorrectlyAsync(int first, int second)
    {
        var sequence1 = AsyncSequence.Return(first);
        var sequence2 = AsyncSequence.Return(second);

        var merged = sequence1.Merge(sequence2);
        Assert.True(merged.FirstAsync().Result <= merged.LastAsync().Result);
    }

    [Fact]
    public Task MergeTwoSequencesToOneAsync()
    {
        var sequence1 = AsyncSequence.Return(1, 2, 4, 7);
        var sequence2 = AsyncSequence.Return(3, 5, 6, 8);
        var expected = AsyncEnumerable.Range(1, 8);

        return AsyncAssert.Equal(expected, sequence1.Merge(sequence2));
    }

    [Fact]
    public Task MergeASequenceOfSequences()
    {
        var sequence1 = AsyncSequence.Return(1, 2, 4, 7);
        var sequence2 = AsyncSequence.Return(3, 5, 6, 8);
        var mergable = ImmutableList<IAsyncEnumerable<int>>.Empty.Add(sequence1).Add(sequence2);
        var expected = AsyncEnumerable.Range(1, 8);

        return AsyncAssert.Equal(expected, mergable.Merge());
    }

    [Fact]
    public Task MergeASequenceWithADifferentComparer()
    {
        var sequence1 = AsyncSequence.Return(7, 4, 2, 1);
        var sequence2 = AsyncSequence.Return(8, 6, 5, 3);
        var expected = AsyncEnumerable.Range(1, 8).Reverse();

        return AsyncAssert.Equal(expected, sequence1.Merge(sequence2, DescendingIntComparer.Create()));
    }
}
