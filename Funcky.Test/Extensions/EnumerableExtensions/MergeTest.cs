// ReSharper disable PossibleMultipleEnumeration

using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MergeTest
{
    [Fact]
    public void MergeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.Merge(doNotEnumerate);
    }

    [Fact]
    public void MergeEmptySequencesResultsInAnEmptySequence()
    {
        var emptySequence = Enumerable.Empty<int>();

        Assert.Empty(emptySequence.Merge(emptySequence, emptySequence, emptySequence));
    }

    [Fact]
    public void MergeAnEmptySequenceWithANonEmptySequenceResultsInTheNonEmptySequence()
    {
        var nonEmptySequence = new List<int> { 1, 2, 4, 7 };
        var emptySequence = Enumerable.Empty<int>();

        Assert.Equal(nonEmptySequence, nonEmptySequence.Merge(emptySequence));
        Assert.Equal(nonEmptySequence, emptySequence.Merge(nonEmptySequence));
    }

    [Property]
    public Property TwoSingleSequencesAreMergedCorrectly(int first, int second)
    {
        var sequence1 = Sequence.Return(first);
        var sequence2 = Sequence.Return(second);

        var merged = sequence1.Merge(sequence2);
        return (merged.First() <= merged.Last()).ToProperty();
    }

    [Fact]
    public void MergeTwoSequencesToOne()
    {
        var sequence1 = new List<int> { 1, 2, 4, 7 };
        var sequence2 = new List<int> { 3, 5, 6, 8 };
        var expected = Enumerable.Range(1, 8);

        Assert.Equal(expected, sequence1.Merge(sequence2));
    }

    [Fact]
    public void MergeASequenceOfSequences()
    {
        var sequence1 = new List<int> { 1, 2, 4, 7 };
        var sequence2 = new List<int> { 3, 5, 6, 8 };
        var mergable = ImmutableList<List<int>>.Empty.Add(sequence1).Add(sequence2);
        var expected = Enumerable.Range(1, 8);

        Assert.Equal(expected, mergable.Merge());
    }

    [Fact]
    public void MergeASequenceWithADifferentComparer()
    {
        var sequence1 = new List<int> { 7, 4, 2, 1 };
        var sequence2 = new List<int> { 8, 6, 5, 3 };
        var expected = Enumerable.Range(1, 8).Reverse();

        Assert.Equal(expected, sequence1.Merge(sequence2, DescendingIntComparer.Create()));
    }
}
