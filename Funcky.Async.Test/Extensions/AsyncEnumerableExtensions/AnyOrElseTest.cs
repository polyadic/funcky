using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class AnyOrElseTest
{
    [Fact]
    public void SourceIsEnumeratedLazily()
    {
        var source = new FailOnEnumerateAsyncSequence<int>();
        _ = source.AnyOrElse(AsyncEnumerable.Empty<int>());
    }

    [Fact]
    public void FallbackIsEnumeratedLazily()
    {
        var source = AsyncEnumerable.Empty<int>().Select(Identity);
        _ = source.AnyOrElse(new FailOnEnumerateAsyncSequence<int>());
    }

    [Fact]
    public async Task IsEmptyWhenBothEnumerablesAreEmpty()
    {
        await AsyncAssert.Empty(AsyncEnumerable.Empty<int>().AnyOrElse(AsyncEnumerable.Empty<int>()));
    }

    [Fact]
    public async Task IsSourceEnumerableWhenNonEmpty()
    {
        var source = AsyncSequence.Return(1, 2, 3);
        var fallback = AsyncSequence.Return(4, 5, 6);
        await AsyncAssert.Equal(source, source.AnyOrElse(fallback));
    }

    [Fact]
    public async Task IsFallbackEnumerableWhenSourceIsEmpty()
    {
        var source = AsyncEnumerable.Empty<int>();
        var fallback = AsyncSequence.Return(1, 2, 3);
        await AsyncAssert.Equal(fallback, source.AnyOrElse(fallback));
    }
}
