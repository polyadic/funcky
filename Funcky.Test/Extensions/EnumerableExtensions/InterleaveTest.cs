#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class InterleaveTest
{
    [Fact]
    public void InterleaveIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.Interleave();
    }

    [Fact]
    public void GivenAnEmptySequenceOfSequencesInterleaveReturnsAnEmptySequence()
    {
        IEnumerable<IEnumerable<int>> emptySequence = [];

        IEnumerable<int> interleaved = emptySequence.Interleave();

        Assert.Empty(interleaved);
    }

    [Fact]
    public void GivenTwoSequencesOfEqualLengthIGetAnInterleavedResult()
    {
        IEnumerable<int> odds = [1, 3, 5, 7, 9, 11];
        IEnumerable<int> evens = [2, 4, 6, 8, 10, 12];
        IEnumerable<int> expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

        var interleaved = odds.Interleave(evens);

        Assert.Equal(expected, interleaved);
    }

    [Fact]
    public void GivenTwoSequencesOfUnequalLengthIGetAnInterleavedResult()
    {
        IEnumerable<int> odds = [1, 3, 5, 7, 9, 11];
        IEnumerable<int> evens = [2, 4, 6];
        IEnumerable<int> expected = [1, 2, 3, 4, 5, 6, 7, 9, 11];

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
        IEnumerable<string> one = [first];
        IEnumerable<string> two = [second];
        IEnumerable<string> three = [third];

        var interleaved = one.Interleave(two, three);

        Assert.Equal([first, second, third], interleaved);
    }

    [Fact]
    public void GivenASequenceOfSequenceTheInnerSequencesGetInterleaved()
    {
        IEnumerable<IEnumerable<int>> sequences = new List<IEnumerable<int>>
        {
            Enumerable.Repeat(1, 2),
            Enumerable.Repeat(42, 2),
        };

        Assert.Equal([1, 42, 1, 42], sequences.Interleave());
    }

    [Fact]
    public void GivenOneSequenceWithElementsAndAllTheOtherSequencesEmptyWeGetTheFirstSequence()
    {
        IEnumerable<int> sequence = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        IEnumerable<int> emptySequence = [];

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
