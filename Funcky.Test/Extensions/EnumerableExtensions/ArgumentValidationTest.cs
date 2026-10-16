#pragma warning disable IDISP005 // The calls only check that the method throws; no result is ever created.

namespace Funcky.Test.Extensions.EnumerableExtensions;

/// <summary>Like LINQ, the iterator-based extensions validate their arguments when they are called, not when the result is enumerated.</summary>
public sealed class ArgumentValidationTest
{
    private static readonly IEnumerable<int> Source = [1, 2, 3];
    private static readonly IEnumerable<int> NullSource = null!;
    private static readonly IEnumerable<IEnumerable<int>> NullSources = null!;

    public static TheoryData<string, Func<object>> CallsWithANullArgument()
        => new()
        {
            { "source", () => NullSource.AdjacentGroupBy(Identity) },
            { "keySelector", () => Source.AdjacentGroupBy((Func<int, int>)null!) },
            { "elementSelector", () => Source.AdjacentGroupBy(Identity, (Func<int, int>)null!) },
            { "resultSelector", () => Source.AdjacentGroupBy(x => x, (Func<int, IEnumerable<int>, int>)null!) },
            { "comparer", () => Source.AdjacentGroupBy(Identity, (IEqualityComparer<int>)null!) },
            { "source", () => NullSource.AnyOrElse(Source) },
            { "fallback", () => Source.AnyOrElse((IEnumerable<int>)null!) },
            { "fallback", () => Source.AnyOrElse((Func<IEnumerable<int>>)null!) },
            { "source", () => NullSource.Chunk(2) },
            { "resultSelector", () => Source.Chunk(2, (Func<IReadOnlyList<int>, int>)null!) },
            { "source", () => NullSource.Inspect(NoOperation) },
            { "inspector", () => Source.Inspect(null!) },
            { "source", () => NullSource.InspectEmpty(NoOperation) },
            { "inspector", () => Source.InspectEmpty(null!) },
            { "source", () => NullSource.Interleave(Source) },
            { "otherSources", () => Source.Interleave(null!) },
            { "source", () => NullSources.Interleave() },
            { "source", () => NullSource.Memoize() },
            { "source1", () => NullSource.Merge(Source) },
            { "source2", () => Source.Merge(NullSource) },
            { "source3", () => Source.Merge(Source, NullSource) },
            { "source4", () => Source.Merge(Source, Source, NullSource) },
            { "sources", () => NullSources.Merge() },
            { "source", () => NullSource.Pairwise() },
            { "resultSelector", () => Source.Pairwise((Func<int, int, int>)null!) },
            { "source", () => NullSource.WhereSelect(Option.Some) },
            { "selector", () => Source.WhereSelect((Func<int, Option<int>>)null!) },
            { "source", () => NullSource.WhereSelect((item, index) => Option.Some(item + index)) },
            { "selector", () => Source.WhereSelect((Func<int, int, Option<int>>)null!) },
            { "source", () => NullSource.PowerSet() },
            { "source", () => NullSource.InclusiveScan(0, (a, b) => a + b) },
            { "accumulator", () => Source.InclusiveScan(0, null!) },
            { "source", () => NullSource.ExclusiveScan(0, (a, b) => a + b) },
            { "accumulator", () => Source.ExclusiveScan(0, null!) },
            { "source", () => NullSource.SlidingWindow(2) },
            { "source", () => NullSource.Split(1) },
            { "source", () => ((int[])null!).Split(1) },
            { "comparer", () => Source.Split(1, null!) },
            { "resultSelector", () => Source.Split(1, EqualityComparer<int>.Default, (Func<IReadOnlyList<int>, int>)null!) },
            { "source", () => NullSource.TakeEvery(2) },
            { "source", () => NullSources.Transpose() },
            { "source", () => NullSource.WithFirst() },
            { "source", () => NullSource.WithLast() },
            { "source", () => NullSource.WithPrevious() },
            { "left", () => NullSource.ZipLongest(Source) },
            { "right", () => Source.ZipLongest(NullSource) },
            { "resultSelector", () => Source.ZipLongest(Source, (Func<EitherOrBoth<int, int>, int>)null!) },
        };

    [Theory]
    [MemberData(nameof(CallsWithANullArgument))]
    public void ANullArgumentIsRejectedWhenTheMethodIsCalled(string parameterName, Func<object> call)
    {
        var exception = Assert.Throws<ArgumentNullException>(call);

        Assert.Equal(parameterName, exception.ParamName);
    }
}
