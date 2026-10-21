#pragma warning disable IDISP005 // The calls only check that the method throws; no result is ever created.
#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

/// <summary>Like LINQ, the iterator-based extensions validate their arguments when they are called, not when the result is enumerated.</summary>
public sealed class ArgumentValidationTest
{
    private static readonly IAsyncEnumerable<int> Source = AsyncSequence.Return(1, 2, 3);
    private static readonly IAsyncEnumerable<int> NullSource = null!;
    private static readonly IEnumerable<IAsyncEnumerable<int>> NullSources = null!;

    public static TheoryData<string, Func<object>> CallsWithANullArgument()
        => new()
        {
            { "source", () => NullSource.AdjacentGroupBy(Identity) },
            { "keySelector", () => Source.AdjacentGroupBy((Func<int, int>)null!) },
            { "elementSelector", () => Source.AdjacentGroupBy(Identity, (Func<int, int>)null!) },
            { "resultSelector", () => Source.AdjacentGroupBy(Identity, (Func<int, IEnumerable<int>, int>)null!) },
            { "comparer", () => Source.AdjacentGroupBy(Identity, (IEqualityComparer<int>)null!) },
            { "source", () => NullSource.AdjacentGroupByAwait(x => ValueTask.FromResult(x)) },
            { "keySelector", () => Source.AdjacentGroupByAwait((Func<int, ValueTask<int>>)null!) },
            { "source", () => NullSource.AdjacentGroupByAwaitWithCancellation((x, _) => ValueTask.FromResult(x)) },
            { "keySelector", () => Source.AdjacentGroupByAwaitWithCancellation((Func<int, CancellationToken, ValueTask<int>>)null!) },
            { "source", () => NullSource.AggregateOrNoneAsync((a, b) => a + b) },
            { "func", () => Source.AggregateOrNoneAsync((Func<int, int, int>)null!) },
            { "source", () => NullSource.AggregateOrNoneAsync((a, b, _) => new ValueTask<int>(a + b)) },
            { "func", () => Source.AggregateOrNoneAsync((Func<int, int, CancellationToken, ValueTask<int>>)null!) },
            { "source", () => NullSource.AnyOrElse(Source) },
            { "fallback", () => Source.AnyOrElse((IAsyncEnumerable<int>)null!) },
            { "fallback", () => Source.AnyOrElse((Func<IAsyncEnumerable<int>>)null!) },
            { "source", () => NullSource.Chunk(2, Identity) },
            { "resultSelector", () => Source.Chunk(2, (Func<IReadOnlyList<int>, int>)null!) },
            { "resultSelector", () => Source.ChunkAwait(2, (Func<IReadOnlyList<int>, ValueTask<int>>)null!) },
            { "resultSelector", () => Source.ChunkAwaitWithCancellation(2, (Func<IReadOnlyList<int>, CancellationToken, ValueTask<int>>)null!) },
            { "source", () => NullSource.Inspect(NoOperation) },
            { "inspector", () => Source.Inspect(null!) },
            { "inspector", () => Source.InspectAwait(null!) },
            { "inspector", () => Source.InspectAwaitWithCancellation(null!) },
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
            { "source", () => NullSource.WhereSelectAwait(item => ValueTask.FromResult(Option.Some(item))) },
            { "selector", () => Source.WhereSelectAwait((Func<int, ValueTask<Option<int>>>)null!) },
            { "source", () => NullSource.WhereSelectAwait((item, index) => ValueTask.FromResult(Option.Some(item + index))) },
            { "selector", () => Source.WhereSelectAwait((Func<int, int, ValueTask<Option<int>>>)null!) },
            { "source", () => NullSource.WhereSelectAwaitWithCancellation((item, _) => ValueTask.FromResult(Option.Some(item))) },
            { "selector", () => Source.WhereSelectAwaitWithCancellation((Func<int, CancellationToken, ValueTask<Option<int>>>)null!) },
            { "source", () => NullSource.WhereSelectAwaitWithCancellation((item, index, _) => ValueTask.FromResult(Option.Some(item + index))) },
            { "selector", () => Source.WhereSelectAwaitWithCancellation((Func<int, int, CancellationToken, ValueTask<Option<int>>>)null!) },
            { "source", () => NullSource.PowerSet() },
            { "source", () => NullSource.InclusiveScan(0, (a, b) => a + b) },
            { "accumulator", () => Source.InclusiveScan(0, (Func<int, int, int>)null!) },
            { "accumulator", () => Source.InclusiveScanAwait(0, (Func<int, int, ValueTask<int>>)null!) },
            { "accumulator", () => Source.InclusiveScanAwaitWithCancellation(0, (Func<int, int, CancellationToken, ValueTask<int>>)null!) },
            { "source", () => NullSource.ExclusiveScan(0, (a, b) => a + b) },
            { "accumulator", () => Source.ExclusiveScan(0, (Func<int, int, int>)null!) },
            { "accumulator", () => Source.ExclusiveScanAwait(0, (Func<int, int, ValueTask<int>>)null!) },
            { "accumulator", () => Source.ExclusiveScanAwaitWithCancellation(0, (Func<int, int, CancellationToken, ValueTask<int>>)null!) },
            { "source", () => NullSource.SlidingWindow(2) },
            { "source", () => NullSource.Split(1) },
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
#endif
