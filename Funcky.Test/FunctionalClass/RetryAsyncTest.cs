#if INTEGRATED_ASYNC
using Funcky.RetryPolicies;
using Funcky.Test.TestUtilities;
using static Funcky.AsyncFunctional;

namespace Funcky.Async.Test.FunctionalClass;

public sealed class RetryAsyncTest
{
    [Fact]
    public async Task ReturnsTheValueImmediatelyIfTheProducerIsPureAndReturnsSome()
    {
        const int value = 10;
        FunctionalAssert.Some(value, await RetryAsync(() => ValueTask.FromResult(Option.Some(value)), new DoNotRetryPolicy()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(90)]
    public async Task RetriesWithDoNotRetryPolicyAlwaysTriesNumberOfRetriesTimes(int numberOfRetries)
    {
        const string produceString = "Hello world!";
        var producer = new OptionProducer<string>(1000, produceString);

        FunctionalAssert.None(await RetryAsync(producer.ProduceAsync, new NoDelayRetryPolicy(numberOfRetries)));
        Assert.Equal(numberOfRetries + 1, producer.Called);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(90)]
    public async Task RetriesWithDoNotRetryRetriesUntilValueProduced(int numberOfRetries)
    {
        const string produceString = "Hello world!";
        var producer = new OptionProducer<string>(numberOfRetries, produceString);

        FunctionalAssert.Some(produceString, await RetryAsync(producer.ProduceAsync, new NoDelayRetryPolicy(1000)));
        Assert.Equal(numberOfRetries + 1, producer.Called);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(90)]
    public async Task RetryAsyncWithoutAnArgumentReturnsAlwaysAValueOrDoesNotReturn(int numberOfRetries)
    {
        const string produceString = "Hello world!";
        var producer = new OptionProducer<string>(numberOfRetries, produceString);

        Assert.Equal(produceString, await RetryAsync(producer.ProduceAsync));
    }

    [Fact]
    public async Task RetryThrowsImmediatelyWhenAlreadyCanceled()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await RetryAsync(() => ValueTask.FromResult(Option<int>.None), source.Token));
    }

    [Fact]
    public async Task RetryThrowsWhenCanceledAfterAnArbitraryNumberOfRetries()
    {
        var delay = TimeSpan.FromMilliseconds(10);

        using var source = new CancellationTokenSource();
        source.CancelAfter(delay * 4);
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await RetryAsync(ProducerWithDelay(delay), source.Token));
    }

    [Fact]
    public async Task RetryThrowsWhenCanceledDuringDelay()
    {
        var delay = TimeSpan.FromMilliseconds(10);
        using var source = new CancellationTokenSource();
        source.CancelAfter(delay);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await RetryAsync(() => ValueTask.FromResult(Option<int>.None), new ConstantDelayPolicy(10, delay), source.Token));
    }

    private static Func<ValueTask<Option<int>>> ProducerWithDelay(TimeSpan delay)
        => async () =>
        {
            await Task.Delay(delay);
            return Option<int>.None;
        };
}
#endif
