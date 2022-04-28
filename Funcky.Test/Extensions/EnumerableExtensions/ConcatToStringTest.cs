namespace Funcky.Test.Extensions.EnumerableExtensions;

public class ConcatToStringTest
{
    [Fact]
    public void ConcatenatingAnEmptySetOfStringsReturnsAnEmptyString()
    {
        var empty = Enumerable.Empty<string>();

        Assert.Equal(string.Empty, empty.ConcatToString());
    }

    [Fact]
    public void ConcatenatingASetWithExactlyOneElementReturnsTheElement()
    {
        var singleElement = Sequence.Return("Alpha");

        Assert.Equal("Alpha", singleElement.ConcatToString());
    }

    [Fact]
    public void ConcatenatingAListOfStringsReturnsAllElementsWithoutASeparator()
    {
        var strings = Sequence.Return("Alpha", "Beta", "Gamma");

        Assert.Equal("AlphaBetaGamma", strings.ConcatToString());
    }

    [Fact]
    public void ConcatenatingNonStringsWorksToo()
    {
        var numbers = Sequence.Return(1, 2, 3);

        Assert.Equal("123", numbers.ConcatToString());
    }

    [Fact]
    public void NullsAreHandledAsEmptyStringsWhileConcatenating()
    {
        var strings = Sequence.Return("Alpha", null, "Gamma");

        Assert.Equal("AlphaGamma", strings.ConcatToString());
    }
}
