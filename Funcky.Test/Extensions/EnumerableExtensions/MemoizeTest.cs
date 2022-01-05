using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MemoizeTest
{
    [Fact]
    public void MemoizeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.Memoize();
    }

    [Fact]
    public void YouCanOnlyEnumerateAFragileSequenceOnce()
    {
        var doNotEnumerateTwice = new DoNotEnumerateTwice<int>(Sequence.Return(1, 42, 100));

        doNotEnumerateTwice.ForEach(NoOperation);

        Assert.Throws<XunitException>(() => doNotEnumerateTwice.ForEach(NoOperation));
    }

    [Fact]
    public void WeCanSafelyEnumerateTwiceOnAFragileSequence()
    {
        var memoized = new DoNotEnumerateTwice<int>(Sequence.Return(1, 42, 100)).Memoize();

        memoized.ForEach(NoOperation);
        memoized.ForEach(NoOperation);
    }

    [Fact]
    public void MemoizeCanPartiallyEnumerate()
    {
        var doNotEnumerateTwice = new DoNotEnumerateTwice<int>(Sequence.Return(1, 42, 100));
        var memoized = doNotEnumerateTwice.Memoize();

        Assert.Equal(0, doNotEnumerateTwice.EnumerationIndex);
        _ = memoized.First();
        Assert.Equal(1, doNotEnumerateTwice.EnumerationIndex);
        _ = memoized.First();
        Assert.Equal(1, doNotEnumerateTwice.EnumerationIndex);
        _ = memoized.Skip(1).First();
        Assert.Equal(2, doNotEnumerateTwice.EnumerationIndex);
        memoized.ForEach(NoOperation);
        Assert.Equal(3, doNotEnumerateTwice.EnumerationIndex);
    }
}
