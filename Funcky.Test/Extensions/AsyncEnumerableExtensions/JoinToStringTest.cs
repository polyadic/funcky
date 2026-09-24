#if INTEGRATED_ASYNC
// ReSharper disable PossibleMultipleEnumeration

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class JoinToStringTest
{
    [Fact]
    public async Task JoiningAnEmptySetOfStringsReturnsAnEmptyString()
    {
        var empty = AsyncEnumerable.Empty<string>();

        Assert.Equal(string.Empty, await empty.JoinToStringAsync(", "));
        Assert.Equal(string.Empty, await empty.JoinToStringAsync(','));
    }

    [Fact]
    public async Task JoiningASetWithExactlyOneElementReturnsTheElementWithoutASeparator()
    {
        var singleElement = AsyncSequence.Return("Alpha");

        Assert.Equal("Alpha", await singleElement.JoinToStringAsync(", "));
        Assert.Equal("Alpha", await singleElement.JoinToStringAsync(','));
    }

    [Fact]
    public async Task JoiningAListOfStringsAddsSeparatorsBetweenTheElements()
    {
        var strings = AsyncSequence.Return("Alpha", "Beta", "Gamma");

        Assert.Equal("Alpha, Beta, Gamma", await strings.JoinToStringAsync(", "));
        Assert.Equal("Alpha,Beta,Gamma", await strings.JoinToStringAsync(','));
    }

    [Fact]
    public async Task JoiningNonStringsReturnASeparatedListToo()
    {
        var numbers = AsyncSequence.Return(1, 2, 3);

        Assert.Equal("1, 2, 3", await numbers.JoinToStringAsync(", "));
        Assert.Equal("1,2,3", await numbers.JoinToStringAsync(','));
    }

    [Fact]
    public async Task NullsAreHandledAsEmptyStringsWhileJoining()
    {
        var strings = AsyncSequence.Return("Alpha", null, "Gamma");

        Assert.Equal("Alpha, , Gamma", await strings.JoinToStringAsync(", "));
        Assert.Equal("Alpha,,Gamma", await strings.JoinToStringAsync(','));
    }
}
#endif
