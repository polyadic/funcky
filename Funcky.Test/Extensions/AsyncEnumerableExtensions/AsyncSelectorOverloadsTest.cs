#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

/// <summary>The single-name overloads with a <c>(…, cancellationToken)</c> delegate, the shape the base class library uses
/// for asynchronous selectors since .NET 10. They replace the <c>…Await</c> and <c>…AwaitWithCancellation</c> overloads.</summary>
public sealed class AsyncSelectorOverloadsTest
{
    [Fact]
    public async Task InspectAwaitsTheInspectorForEachElement()
    {
        var inspected = new List<int>();

        var result = await AsyncSequence.Return(1, 2, 3)
            .Inspect(async (item, _) =>
            {
                await Task.Yield();
                inspected.Add(item);
            })
            .ToListAsync();

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(new[] { 1, 2, 3 }, inspected);
    }

    [Fact]
    public async Task TheDelegateReceivesTheTokenOfTheEnumeration()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var tokens = new List<CancellationToken>();

        var inspected = AsyncSequence.Return(1, 2).Inspect(async (_, token) =>
        {
            await Task.Yield();
            tokens.Add(token);
        });

        await foreach (var _ in inspected.WithCancellation(cancellationToken))
        {
        }

        Assert.Equal(2, tokens.Count);
        Assert.All(tokens, token => Assert.Equal(cancellationToken, token));
    }

    [Fact]
    public async Task ChunkAwaitsTheResultSelector()
    {
        var sums = await AsyncSequence.Return(1, 2, 3, 4, 5)
            .Chunk(2, (chunk, _) => ValueTask.FromResult(chunk.Sum()))
            .ToListAsync();

        Assert.Equal(new[] { 3, 7, 5 }, sums);
    }

    [Fact]
    public async Task InclusiveScanAwaitsTheAccumulator()
    {
        var prefixSums = await AsyncSequence.Return(1, 2, 3)
            .InclusiveScan(0, (sum, item, _) => ValueTask.FromResult(sum + item))
            .ToListAsync();

        Assert.Equal(new[] { 1, 3, 6 }, prefixSums);
    }

    [Fact]
    public async Task ExclusiveScanAwaitsTheAccumulator()
    {
        var prefixSums = await AsyncSequence.Return(1, 2, 3)
            .ExclusiveScan(0, (sum, item, _) => ValueTask.FromResult(sum + item))
            .ToListAsync();

        Assert.Equal(new[] { 0, 1, 3 }, prefixSums);
    }

    [Fact]
    public async Task WhereSelectAwaitsTheSelector()
    {
        var squaresOfEvenNumbers = await AsyncSequence.Return(1, 2, 3, 4)
            .WhereSelect((item, _) => ValueTask.FromResult(item % 2 == 0 ? Option.Some(item * item) : Option<int>.None))
            .ToListAsync();

        Assert.Equal(new[] { 4, 16 }, squaresOfEvenNumbers);
    }

    [Fact]
    public async Task WhereSelectWithIndexAwaitsTheSelector()
    {
        var itemsAtEvenIndices = await AsyncSequence.Return(10, 11, 12, 13)
            .WhereSelect((item, index, _) => ValueTask.FromResult(index % 2 == 0 ? Option.Some(item) : Option<int>.None))
            .ToListAsync();

        Assert.Equal(new[] { 10, 12 }, itemsAtEvenIndices);
    }

    [Fact]
    public async Task AdjacentGroupByAwaitsTheKeySelector()
    {
        var keys = await AsyncSequence.Return(1, 2, 11, 12, 3)
            .AdjacentGroupBy((item, _) => ValueTask.FromResult(item / 10))
            .Select(group => group.Key)
            .ToListAsync();

        Assert.Equal(new[] { 0, 1, 0 }, keys);
    }

    [Fact]
    public async Task AdjacentGroupByAwaitsAllSelectors()
    {
        var results = await AsyncSequence.Return(1, 2, 11, 12, 3)
            .AdjacentGroupBy(
                (item, _) => ValueTask.FromResult(item / 10),
                (item, _) => ValueTask.FromResult(item * 2),
                (key, elements, _) => ValueTask.FromResult($"{key}:{elements.Sum()}"))
            .ToListAsync();

        Assert.Equal(new[] { "0:6", "1:46", "0:6" }, results);
    }

    [Fact]
    public async Task AdjacentGroupByWithComparerAwaitsTheSelectors()
    {
        var keys = await AsyncSequence.Return("a", "A", "b")
            .AdjacentGroupBy((item, _) => ValueTask.FromResult(item), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Key)
            .ToListAsync();

        Assert.Equal(new[] { "a", "b" }, keys);
    }

    [Fact]
    public async Task FirstOrNoneAwaitsThePredicate()
    {
        FunctionalAssert.Some(2, await AsyncSequence.Return(1, 2, 3).FirstOrNoneAsync((item, _) => ValueTask.FromResult(item > 1)));
        FunctionalAssert.None(await AsyncSequence.Return(1, 2, 3).FirstOrNoneAsync((item, _) => ValueTask.FromResult(item > 3)));
    }

    [Fact]
    public async Task LastSingleAndNoneAwaitThePredicate()
    {
        FunctionalAssert.Some(3, await AsyncSequence.Return(1, 2, 3).LastOrNoneAsync((item, _) => ValueTask.FromResult(item > 1)));
        FunctionalAssert.Some(2, await AsyncSequence.Return(1, 2, 3).SingleOrNoneAsync((item, _) => ValueTask.FromResult(item == 2)));
        Assert.True(await AsyncSequence.Return(1, 2, 3).NoneAsync((item, _) => ValueTask.FromResult(item > 3)));
        Assert.False(await AsyncSequence.Return(1, 2, 3).NoneAsync((item, _) => ValueTask.FromResult(item > 2)));
    }

    [Fact]
    public async Task MaxMinAndAverageAwaitTheSelector()
    {
        FunctionalAssert.Some(6, await AsyncSequence.Return(1, 2, 3).MaxOrNoneAsync((item, _) => ValueTask.FromResult(item * 2)));
        FunctionalAssert.Some(2, await AsyncSequence.Return(1, 2, 3).MinOrNoneAsync((item, _) => ValueTask.FromResult(item * 2)));
        FunctionalAssert.Some(3, await AsyncSequence.Return(1, 2, 3).MaxOrNoneAsync((item, _) => ValueTask.FromResult(item == 2 ? Option<int>.None : Option.Some(item))));
        FunctionalAssert.Some(4.0, await AsyncSequence.Return(1, 2, 3).AverageOrNoneAsync((item, _) => ValueTask.FromResult(item * 2)));
        FunctionalAssert.Some(2.0, await AsyncSequence.Return(1, 2, 3).AverageOrNoneAsync((item, _) => ValueTask.FromResult(item == 2 ? Option<long>.None : Option.Some((long)item))));
        FunctionalAssert.None(await AsyncSequence.Return<int>().AverageOrNoneAsync((item, _) => ValueTask.FromResult((decimal)item)));
    }

    [Fact]
    public async Task PartitionAwaitsThePredicateAndTheResultSelector()
    {
        var (evens, odds) = await AsyncSequence.Return(1, 2, 3, 4).PartitionAsync((item, _) => ValueTask.FromResult(item % 2 == 0));
        Assert.Equal(new[] { 2, 4 }, evens);
        Assert.Equal(new[] { 1, 3 }, odds);

        var summary = await AsyncSequence.Return(1, 2, 3, 4).PartitionAsync(
            (item, _) => ValueTask.FromResult(item % 2 == 0),
            (left, right, _) => ValueTask.FromResult($"{left.Count}:{right.Count}"));
        Assert.Equal("2:2", summary);
    }

    [Fact]
    public async Task TheTerminalDelegatesReceiveTheTokenOfTheCall()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var tokens = new List<CancellationToken>();

        _ = await AsyncSequence.Return(1, 2).FirstOrNoneAsync(
            (_, token) =>
            {
                tokens.Add(token);
                return ValueTask.FromResult(false);
            },
            cancellationToken);
        _ = await AsyncSequence.Return(1, 2).PartitionAsync(
            (_, token) => ValueTask.FromResult(token == cancellationToken),
            (left, right, token) =>
            {
                tokens.Add(token);
                return ValueTask.FromResult(left.Count + right.Count);
            },
            cancellationToken);

        Assert.Equal(3, tokens.Count);
        Assert.All(tokens, token => Assert.Equal(cancellationToken, token));
    }
}
#endif
