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
        FunctionalAssert.Some(10, await Option.Some(Task.FromResult(10)));
        FunctionalAssert.Some(10, await Option.Some(Task.FromResult(10)).ConfigureAwait(false));
        FunctionalAssert.None(await Option<Task<int>>.None.ConfigureAwait(false));
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
        FunctionalAssert.Some(10, await Option.Some(ValueTask.FromResult(10)));
        FunctionalAssert.Some(10, await Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false));
        FunctionalAssert.None(await Option<ValueTask<int>>.None);
        FunctionalAssert.None(await Option<ValueTask<int>>.None.ConfigureAwait(false));
    }

    [Fact]
    public void OnCompletedInvokesTheContinuationForNone()
    {
        AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    [Fact]
    public void UnsafeOnCompletedInvokesTheContinuationForNone()
    {
        AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void OnCompletedInvokesTheContinuationForSome()
    {
        AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    private static void AssertContinuationIsInvoked(Action<Action> registerContinuation)
    {
        // Awaiters are allowed to schedule the continuation asynchronously.
        using var invoked = new ManualResetEventSlim();
        registerContinuation(invoked.Set);
        Assert.True(invoked.Wait(TimeSpan.FromSeconds(5)), "the continuation was not invoked");
    }
}
