using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class InterleaveTest
    {
        [Fact]
        public void GivenAnEmptySequenceOfSequencesInterleaveReturnsAnEmptySequence()
        {
            IEnumerable<IEnumerable<int>> emptySequence = Enumerable.Empty<IEnumerable<int>>();

            IEnumerable<int> interleaved = emptySequence.Interleave();

            Assert.Empty(interleaved);
        }

        [Fact]
        public void GivenTwoSequencesOfEqualLengthIGetAnInterleavedResult()
        {
            IEnumerable<int> odds = new List<int> { 1, 3, 5, 7, 9, 11 };
            IEnumerable<int> evens = new List<int> { 2, 4, 6, 8, 10, 12 };
            IEnumerable<int> expected = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            IEnumerable<int> interleaved = odds.Interleave(evens);

            Assert.Equal(expected, interleaved);
        }

        [Fact]
        public void GivenTwoSequencesOfUnequalLengthIGetAnInterleavedResult()
        {
            IEnumerable<int> odds = new List<int> { 1, 3, 5, 7, 9, 11 };
            IEnumerable<int> evens = new List<int> { 2, 4, 6 };
            IEnumerable<int> expected = new List<int> { 1, 2, 3, 4, 5, 6, 7, 9, 11 };

            IEnumerable<int> interleaved = odds.Interleave(evens);

            Assert.Equal(expected, interleaved);
        }

        [Theory]
        [InlineData("a", "b", "c")]
        [InlineData("a", "c", "b")]
        [InlineData("b", "a", "c")]
        [InlineData("b", "c", "a")]
        [InlineData("c", "a", "b")]
        [InlineData("c", "b", "a")]
        public void GivenMultipleSequencesTheOrderIsPreserved(string first, string second, string third)
        {
            IEnumerable<string> one = Enumerable.Repeat(first, 1);
            IEnumerable<string> two = Enumerable.Repeat(second, 1);
            IEnumerable<string> three = Enumerable.Repeat(third, 1);

            var interleaved = one.Interleave(two, three);

            Assert.Equal(new List<string> { first, second, third }, interleaved);
        }

        [Fact]
        public void GivenASequenceOfSequenceTheInnerSequencesGetInterleaved()
        {
            IEnumerable<IEnumerable<int>> sequences = new List<IEnumerable<int>>
            {
                Enumerable.Repeat(1, 2),
                Enumerable.Repeat(42, 2),
            };

            Assert.Equal(new List<int> { 1, 42, 1, 42 }, sequences.Interleave());
        }

        [Fact]
        public void GivenOneSequenceWithElementsAndAllTheOtherSequencesEmptyWeGetTheFirstSequence()
        {
            IEnumerable<int> sequence = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            IEnumerable<int> emptySequence = Enumerable.Empty<int>();

            Assert.Equal(sequence, emptySequence.Interleave(emptySequence, sequence, emptySequence));
        }

        [Fact]
        public void GivenASequenceOfSequencesInterleaveReturnsTheExpectedSequence()
        {
            var sequences = new List<IEnumerable<int>>()
            {
                Enumerable.Repeat(1, 10),
                Enumerable.Repeat(2, 10),
                Enumerable.Repeat(3, 10),
                Enumerable.Repeat(4, 10),
            };

            Assert.Equal(sequences.Select(s => s.Count()).Sum(), sequences.Interleave().Count());

            int expected = 1;
            foreach (var element in sequences.Interleave())
            {
                Assert.Equal(expected, element);
                expected = (expected % 4) + 1;
            }
        }
    }
}
