// ReSharper disable PossibleMultipleEnumeration

using Funcky.Async.Extensions;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class JoinToStringTest
{
    [Fact]
    public async Task JoiningAnEmptySetOfStringsReturnsAnEmptyString()
    {
        var empty = AsyncEnumerable.Empty<string>();

        Assert.Equal(string.Empty, await empty.JoinToString(", "));
        Assert.Equal(string.Empty, await empty.JoinToString(','));
    }

    [Fact]
    public async Task JoiningASetWithExactlyOneElementReturnsTheElementWithoutASeparator()
    {
        var singleElement = AsyncSequence.Return("Alpha");

        Assert.Equal("Alpha", await singleElement.JoinToString(", "));
        Assert.Equal("Alpha", await singleElement.JoinToString(','));
    }

    [Fact]
    public async Task JoiningAListOfStringsAddsSeparatorsBetweenTheElements()
    {
        var strings = AsyncSequence.Return("Alpha", "Beta", "Gamma");

        Assert.Equal("Alpha, Beta, Gamma", await strings.JoinToString(", "));
        Assert.Equal("Alpha,Beta,Gamma", await strings.JoinToString(','));
    }

    [Fact]
    public async Task JoiningNonStringsReturnASeparatedListToo()
    {
        var numbers = AsyncSequence.Return(1, 2, 3);

        Assert.Equal("1, 2, 3", await numbers.JoinToString(", "));
        Assert.Equal("1,2,3", await numbers.JoinToString(','));
    }

    [Fact]
    public async Task NullsAreHandledAsEmptyStringsWhileJoining()
    {
        var strings = AsyncSequence.Return("Alpha", null, "Gamma");

        Assert.Equal("Alpha, , Gamma", await strings.JoinToString(", "));
        Assert.Equal("Alpha,,Gamma", await strings.JoinToString(','));
    }
}
