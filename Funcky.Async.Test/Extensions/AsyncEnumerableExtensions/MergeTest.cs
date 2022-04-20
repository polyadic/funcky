// ReSharper disable PossibleMultipleEnumeration

using System.Collections.Immutable;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Funcky.Test.TestUtils;
using Xunit;

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
    public async Task MergeEmptySequencesResultsInAnEmptySequence()
    {
        var emptySequence = AsyncEnumerable.Empty<int>();

        await AsyncAssert.Empty(emptySequence.Merge(emptySequence, emptySequence, emptySequence));
    }

    [Fact]
    public async Task MergeAnEmptySequenceWithANonEmptySequenceResultsInTheNonEmptySequenceAsync()
    {
        var nonEmptySequence = new List<int> { 1, 2, 4, 7 }.ToAsyncEnumerable();
        var emptySequence = AsyncEnumerable.Empty<int>();

        await AsyncAssert.Equal(nonEmptySequence, nonEmptySequence.Merge(emptySequence));
        await AsyncAssert.Equal(nonEmptySequence, emptySequence.Merge(nonEmptySequence));
    }

    [Property]
    public void TwoSingleSequencesAreMergedCorrectlyAsync(int first, int second)
    {
        var sequence1 = Sequence.Return(first).ToAsyncEnumerable();
        var sequence2 = Sequence.Return(second).ToAsyncEnumerable();

        var merged = sequence1.Merge(sequence2);
        Assert.True(merged.FirstAsync().Result <= merged.LastAsync().Result);
    }

    [Fact]
    public async Task MergeTwoSequencesToOneAsync()
    {
        var sequence1 = new List<int> { 1, 2, 4, 7 }.ToAsyncEnumerable();
        var sequence2 = new List<int> { 3, 5, 6, 8 }.ToAsyncEnumerable();
        var expected = AsyncEnumerable.Range(1, 8);

        await AsyncAssert.Equal(expected, sequence1.Merge(sequence2));
    }

    [Fact]
    public async Task MergeASequenceOfSequences()
    {
        var sequence1 = new List<int> { 1, 2, 4, 7 }.ToAsyncEnumerable();
        var sequence2 = new List<int> { 3, 5, 6, 8 }.ToAsyncEnumerable();
        var mergable = ImmutableList<IAsyncEnumerable<int>>.Empty.Add(sequence1).Add(sequence2);
        var expected = AsyncEnumerable.Range(1, 8);

        await AsyncAssert.Equal(expected, mergable.Merge());
    }

    [Fact]
    public async Task MergeASequenceWithADifferentComparer()
    {
        var sequence1 = new List<int> { 7, 4, 2, 1 }.ToAsyncEnumerable();
        var sequence2 = new List<int> { 8, 6, 5, 3 }.ToAsyncEnumerable();
        var expected = AsyncEnumerable.Range(1, 8).Reverse();

        await AsyncAssert.Equal(expected, sequence1.Merge(sequence2, DescendingIntComparer.Create()));
    }
}
