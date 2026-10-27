#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.Monads;

public sealed class OptionAwaiterTest
{
    private static readonly TimeSpan ContinuationTimeout = TimeSpan.FromSeconds(5);

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
    public async Task OptionOfIncompleteTaskIsAwaitable()
    {
        var source = new TaskCompletionSource<int>();
        var awaiting = AwaitAsync(Option.Some(source.Task));
        source.SetResult(10);
        FunctionalAssert.Some(10, await awaiting);

        static async Task<Option<int>> AwaitAsync(Option<Task<int>> option) => await option;
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
        AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    [Fact]
    public void UnsafeOnCompletedInvokesTheContinuationForSome()
    {
        AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvoked(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void OnCompletedForSomePostsTheContinuationToTheCurrentSynchronizationContext()
    {
        AssertContinuationIsPosted(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(Task.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(ValueTask.FromResult(10)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(Task.CompletedTask).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(Task.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option.Some(ValueTask.CompletedTask).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void ConfigureAwaitFalseForSomeBypassesTheCurrentSynchronizationContext()
    {
        AssertContinuationIsNotPosted(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void ContinuationForNoneIsNeverPosted()
    {
        AssertContinuationIsNotPosted(continuation => Option<Task<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option<Task<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option<ValueTask<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted(continuation => Option<ValueTask<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
    }

    // The awaiter of a completed Task posts its continuation to the current SynchronizationContext instead of
    // invoking it inline. Under xunit that is the MaxConcurrencySyncContext with one worker thread per core,
    // which other tests can occupy for seconds, so a test waiting for such a continuation starves on small
    // runners (#1004). Without a context the continuation goes to the thread pool, which cannot be starved by
    // other tests, so the wait below is both safe and short.
    private static void AssertContinuationIsInvoked(Action<Action> registerContinuation)
        => WithoutSynchronizationContext(() =>
        {
            using var invoked = new ManualResetEventSlim();
            registerContinuation(invoked.Set);
            Assert.True(invoked.Wait(ContinuationTimeout), "the continuation was not invoked");
        });

    // A recording context runs posted callbacks inline, so a posted continuation has run when the registration returns.
    private static void AssertContinuationIsPosted(Action<Action> registerContinuation)
        => WithSynchronizationContext(new RecordingSynchronizationContext(), context =>
        {
            var invoked = false;
            registerContinuation(() => invoked = true);
            Assert.Equal(1, context.Posted);
            Assert.True(invoked, "the continuation was not invoked");
        });

    private static void AssertContinuationIsNotPosted(Action<Action> registerContinuation)
        => WithSynchronizationContext(new RecordingSynchronizationContext(), context =>
        {
            using var invoked = new ManualResetEventSlim();
            registerContinuation(invoked.Set);
            Assert.True(invoked.Wait(ContinuationTimeout), "the continuation was not invoked");
            Assert.Equal(0, context.Posted);
        });

    private static void WithoutSynchronizationContext(Action action)
    {
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
        try
        {
            action();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }
    }

    private static void WithSynchronizationContext<TContext>(TContext context, Action<TContext> action)
        where TContext : SynchronizationContext
    {
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(context);
        try
        {
            action(context);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }
    }

    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        private int _posted;

        public int Posted => _posted;

        public override void Post(SendOrPostCallback d, object? state)
        {
            Interlocked.Increment(ref _posted);
            d(state);
        }
    }
}
#endif
