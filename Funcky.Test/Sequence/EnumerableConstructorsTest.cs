#pragma warning disable λ1202 // This test exercises the classic Sequence form itself.

namespace Funcky.Test;

public sealed class EnumerableConstructorsTest
{
    [Fact]
    public void ReturnIsAvailableOnEnumerable()
    {
        Assert.Equal(Sequence.Return(42), Enumerable.Return(42));
        Assert.Equal(42, Assert.Single(Enumerable.Return(42)));
    }

    [Fact]
    public void ReturnWithSeveralElementsIsAvailableOnEnumerable()
    {
        Assert.Equal([1, 2, 3], Enumerable.Return(1, 2, 3));
        Assert.Empty(Enumerable.Return<int>());
    }

    [Fact]
    public void ConcatIsAvailableOnEnumerableForAnyNumberOfSequences()
    {
        IEnumerable<int> a = [1], b = [2], c = [3];

        Assert.Equal([1, 2, 3], Enumerable.Concat(a, b, c));
        Assert.Equal([1, 2, 3], Enumerable.Concat([a, b, c]));
        Assert.Equal([1, 2], Enumerable.Concat(a, b));
        Assert.Empty(Enumerable.Concat<int>());
    }

    [Fact]
    public void FromNullableIsAvailableOnEnumerable()
    {
        Assert.Empty(Enumerable.FromNullable((string?)null));
        Assert.Empty(Enumerable.FromNullable((int?)null));
        Assert.Equal(["x"], Enumerable.FromNullable("x"));
        Assert.Equal([3], Enumerable.FromNullable((int?)3));
    }

    [Fact]
    public void SuccessorsIsAvailableOnEnumerableInAllFourShapes()
    {
        Assert.Equal([1, 2, 3], Enumerable.Successors(1, n => n + 1).Take(3));
        Assert.Equal([1, 2, 3], Enumerable.Successors(Option.Some(1), n => n + 1).Take(3));
        Assert.Equal([1, 2], Enumerable.Successors(1, n => n < 2 ? Option.Some(n + 1) : Option<int>.None));
        Assert.Empty(Enumerable.Successors(Option<int>.None, n => Option.Some(n + 1)));
    }

    [Fact]
    public void CycleIsAvailableOnEnumerable()
        => Assert.Equal([7, 7, 7], Enumerable.Cycle(7).Take(3));

    [Fact]
    public void CycleRangeIsAvailableOnEnumerable()
    {
        using var cycle = Enumerable.CycleRange([1, 2]);
        Assert.Equal([1, 2, 1, 2, 1], cycle.Take(5));
    }

    [Fact]
    public void CycleMaterializedIsAvailableOnEnumerable()
        => Assert.Equal([1, 2, 1], Enumerable.CycleMaterialized([1, 2]).Take(3));

    [Fact]
    public void RepeatRangeIsAvailableOnEnumerable()
    {
        using var repeated = Enumerable.RepeatRange([1, 2], 2);
        Assert.Equal([1, 2, 1, 2], repeated);
    }

    [Fact]
    public void RepeatMaterializedIsAvailableOnEnumerable()
        => Assert.Equal([1, 2, 1, 2], Enumerable.RepeatMaterialized([1, 2], 2));

    [Fact]
    public void TheBuiltInConstructorsOfEnumerableStillBind()
    {
        Assert.Equal([0, 1, 2], Enumerable.Range(0, 3));
        Assert.Equal([5, 5], Enumerable.Repeat(5, 2));
        Assert.Empty(Enumerable.Empty<int>());
    }
}
