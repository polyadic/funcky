using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class MergeTest
    {
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

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(-42, 42)]
        [InlineData(42, -42)]
        [InlineData(100, 1000)]
        [InlineData(1000, 100)]
        public void TwoSingleSequencesAreMergedCorrectly(int first, int second)
        {
            var sequence1 = Sequence.Return(first);
            var sequence2 = Sequence.Return(second);

            var merged = sequence1.Merge(sequence2);
            Assert.True(merged.First() < merged.Last());
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

            Assert.Equal(expected, sequence1.Merge(sequence2, Option.Some<IComparer<int>>(DescendingIntComparer.Create())));
        }
    }
}
