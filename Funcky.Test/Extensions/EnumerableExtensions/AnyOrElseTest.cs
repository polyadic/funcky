using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class AnyOrElseTest
{
    [Fact]
    public void IsEmptyWhenBothEnumerablesAreEmpty()
    {
        Assert.Empty(Enumerable.Empty<int>().AnyOrElse(Enumerable.Empty<int>()));
    }

    [Fact]
    public void IsSourceEnumerableWhenNonEmpty()
    {
        var source = Sequence.Return(1, 2, 3).PreventLinqOptimizations();
        var fallback = Sequence.Return(4, 5, 6);
        Assert.Equal(source, source.AnyOrElse(fallback));
    }

    [Fact]
    public void IsFallbackEnumerableWhenSourceIsEmpty()
    {
        var source = Enumerable.Empty<int>().PreventLinqOptimizations();
        var fallback = Sequence.Return(1, 2, 3);
        Assert.Equal(fallback, source.AnyOrElse(fallback));
    }

    [Fact]
    public void SourceIsEnumeratedLazily()
    {
        var source = new FailOnEnumerationSequence<int>();
        _ = source.AnyOrElse(Enumerable.Empty<int>());
    }

    [Fact]
    public void FallbackIsEnumeratedLazily()
    {
        var source = Enumerable.Empty<int>().PreventLinqOptimizations();
        _ = source.AnyOrElse(new FailOnEnumerationSequence<int>());
    }

    [Fact]
    public void ReturnsSourceDirectlyWhenSourceIsNonEmptyCollection()
    {
        var source = new FailOnEnumerationList(length: 1);
        var fallback = Enumerable.Empty<int>();
        Assert.Same(source, source.AnyOrElse(fallback));
    }

    [Fact]
    public void ReturnsFallbackDirectlyWhenSourceIsEmptyCollection()
    {
        var source = new FailOnEnumerationList(length: 0);
        var fallback = Enumerable.Empty<int>();
        Assert.Same(fallback, source.AnyOrElse(fallback));
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void ReturnsSourceDirectlyWhenSourceIsNonEmptyAndNonEnumeratedCountIsKnown()
    {
        var source = new FailOnEnumerationList(length: 1).Select(Identity);
        var fallback = Enumerable.Empty<int>();
        Assert.Same(source, source.AnyOrElse(fallback));
    }

    [Fact]
    public void ReturnsFallbackDirectlyWhenSourceIsEmptyAndNonEnumeratedCountIsKnown()
    {
        var source = new FailOnEnumerationList(length: 0).Select(Identity);
        var fallback = Enumerable.Empty<int>();
        Assert.Same(fallback, source.AnyOrElse(fallback));
    }
#endif
}
