using Funcky.Async.Monads;

namespace Funcky.Async.Test.Monads;

public sealed class OptionAsyncTest
{
    [Fact]
    public async Task OptionOfTaskCanBeAwaited()
    {
        var delay = TimeSpan.FromMilliseconds(100);

        await Option<Task>.None;
        await Option.Some(Task.Delay(delay));

        Assert.Equal(
            Option.Some(10),
            await Option.Some(DelayedResult(10, delay)));
        Assert.Equal(
            Option<int>.None,
            await Option<Task<int>>.None);
    }

    [Fact]
    public async Task OptionOfValueTaskCanBeAwaited()
    {
        var delay = TimeSpan.FromMilliseconds(100);

        await Option<ValueTask>.None;
        await Option.Some(new ValueTask(Task.Delay(delay)));

        Assert.Equal(
            Option.Some(10),
            await Option.Some(new ValueTask<int>(DelayedResult(10, delay))));
        Assert.Equal(
            Option<int>.None,
            await Option<ValueTask<int>>.None);
    }

    private static async Task<T> DelayedResult<T>(T value, TimeSpan delay)
    {
        await Task.Delay(delay);
        return value;
    }
}
