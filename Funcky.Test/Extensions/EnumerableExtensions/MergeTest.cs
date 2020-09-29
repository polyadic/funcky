using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class MergeTest
    {
        [Fact]
        public void MergeEmptySequncesResultsInAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<int>();

            Assert.Empty(emptySequence.Merge(Comparer<int>.Default, emptySequence, emptySequence, emptySequence));
        }

        [Fact]
        public void MergeTwoSequencesToOne()
        {
            var asc1 = new List<int> { 1, 2, 4, 7 };
            var asc2 = new List<int> { 3, 5, 6, 8 };
            var expected = Enumerable.Range(1, 8);

            Assert.Equal(expected, asc1.Merge(Comparer<int>.Default, asc2));
        }
    }
}
