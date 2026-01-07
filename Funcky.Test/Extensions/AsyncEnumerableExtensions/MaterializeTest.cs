using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MaterializeTest
{
    [Fact]
    public async Task MaterializeEnumeratesNonCollection()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        await Assert.ThrowsAsync<XunitException>(async () => await doNotEnumerate.MaterializeAsync());
    }

    [Fact]
    public async Task MaterializeASequenceReturnsAListByDefault()
    {
        var sequence = AsyncEnumerable.Repeat("Hello world!", 3);

        Assert.IsType<List<string>>(await sequence.MaterializeAsync());
    }

    [Fact]
    public async Task MaterializeWithMaterializationReturnsCorrectCollectionWhenEnumerate()
    {
        var sequence = AsyncEnumerable.Repeat("Hello world!", 3);

        Assert.IsType<HashSet<string>>(await sequence.MaterializeAsync(ToHashSet));
    }

    private static ValueTask<HashSet<string>> ToHashSet(IAsyncEnumerable<string> sequence, CancellationToken cancellationToken)
        => sequence.ToHashSetAsync(cancellationToken);
}
