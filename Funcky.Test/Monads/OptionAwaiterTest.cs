#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.Monads;

public sealed class OptionAwaiterTest
{
    [Fact]
    public async Task OptionOfVoidTaskIsAwaitable()
    {
        await Option.Some(Task.Delay(10));
        await Option.Some(Task.Delay(10)).ConfigureAwait(false);
        await Option<Task>.None;
        await Option<Task>.None.ConfigureAwait(false);
    }

    [Fact]
    public async Task OptionOfTaskIsAwaitable()
    {
        Assert.Some(10, await Option.Some(Task.FromResult(10)));
        Assert.Some(10, await Option.Some(Task.FromResult(10)).ConfigureAwait(false));
        Assert.None(await Option<Task<int>>.None.ConfigureAwait(false));
    }

    [Fact]
    public async Task OptionOfVoidValueTaskIsAwaitable()
    {
        await Option.Some(new ValueTask(Task.Delay(10)));
        await Option.Some(new ValueTask(Task.Delay(10))).ConfigureAwait(false);
        await Option<ValueTask>.None;
        await Option<ValueTask>.None.ConfigureAwait(false);
    }

    [Fact]
    public async Task OptionOfValueTaskIsAwaitable()
    {
        Assert.Some(10, await Option.Some(ValueTask.FromResult(10)));
        Assert.Some(10, await Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false));
        Assert.None(await Option<ValueTask<int>>.None);
        Assert.None(await Option<ValueTask<int>>.None.ConfigureAwait(false));
    }

    [Fact]
    public async Task OnCompletedInvokesTheContinuationForNone()
    {
        await AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task>.None.GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    [Fact]
    public async Task UnsafeOnCompletedInvokesTheContinuationForNone()
    {
        await AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public async Task OnCompletedInvokesTheContinuationForSome()
    {
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().OnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    [Fact]
    public async Task UnsafeOnCompletedInvokesTheContinuationForSome()
    {
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
        await AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    private static async Task AssertContinuationIsInvoked(Action<Action> registerContinuation)
    {
        // Awaiters are allowed to schedule the continuation asynchronously. A completed Task posts it to the
        // current synchronization context, which xunit limits to one thread per core, so this must not block
        // that thread while waiting: a blocked thread would starve the very continuation it waits for.
        var invoked = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        registerContinuation(() => invoked.TrySetResult(true));
        var completed = await Task.WhenAny(invoked.Task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.True(completed == invoked.Task, "the continuation was not invoked");
    }
}
#endif
