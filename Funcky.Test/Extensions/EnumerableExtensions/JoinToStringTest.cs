// ReSharper disable PossibleMultipleEnumeration

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class JoinToStringTest
{
    [Fact]
    public void JoiningAnEmptySetOfStringsReturnsAnEmptyString()
    {
        var empty = Enumerable.Empty<string>();

        Assert.Equal(string.Empty, empty.JoinToString(", "));
        Assert.Equal(string.Empty, empty.JoinToString(','));
    }

    [Fact]
    public void JoiningASetWithExactlyOneElementReturnsTheElementWithoutASeparator()
    {
        var singleElement = Sequence.Return("Alpha");

        Assert.Equal("Alpha", singleElement.JoinToString(", "));
        Assert.Equal("Alpha", singleElement.JoinToString(','));
    }

    [Fact]
    public void JoiningAListOfStringsAddsSeparatorsBetweenTheElements()
    {
        var strings = Sequence.Return("Alpha", "Beta", "Gamma");

        Assert.Equal("Alpha, Beta, Gamma", strings.JoinToString(", "));
        Assert.Equal("Alpha,Beta,Gamma", strings.JoinToString(','));
    }

    [Fact]
    public void JoiningNonStringsReturnASeparatedListToo()
    {
        var numbers = Sequence.Return(1, 2, 3);

        Assert.Equal("1, 2, 3", numbers.JoinToString(", "));
        Assert.Equal("1,2,3", numbers.JoinToString(','));
    }

    [Fact]
    public void NullsAreHandledAsEmptyStringsWhileJoining()
    {
        var strings = Sequence.Return("Alpha", null, "Gamma");

        Assert.Equal("Alpha, , Gamma", strings.JoinToString(", "));
        Assert.Equal("Alpha,,Gamma", strings.JoinToString(','));
    }
}
