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

#pragma warning disable xUnit1031 // GetResult on the awaiter of None or of an already completed task returns immediately.
    [Fact]
    public void AwaiterOfNoneIsCompletedWithNone()
    {
        var taskAwaiter = Option<Task<int>>.None.GetAwaiter();
        Assert.True(taskAwaiter.IsCompleted);
        FunctionalAssert.None(taskAwaiter.GetResult());

        var configuredTaskAwaiter = Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredTaskAwaiter.IsCompleted);
        FunctionalAssert.None(configuredTaskAwaiter.GetResult());

        var valueTaskAwaiter = Option<ValueTask<int>>.None.GetAwaiter();
        Assert.True(valueTaskAwaiter.IsCompleted);
        FunctionalAssert.None(valueTaskAwaiter.GetResult());

        var configuredValueTaskAwaiter = Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredValueTaskAwaiter.IsCompleted);
        FunctionalAssert.None(configuredValueTaskAwaiter.GetResult());
    }

    [Fact]
    public void AwaiterOfVoidNoneIsCompleted()
    {
        var taskAwaiter = Option<Task>.None.GetAwaiter();
        Assert.True(taskAwaiter.IsCompleted);
        taskAwaiter.GetResult();

        var configuredTaskAwaiter = Option<Task>.None.ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredTaskAwaiter.IsCompleted);
        configuredTaskAwaiter.GetResult();

        var valueTaskAwaiter = Option<ValueTask>.None.GetAwaiter();
        Assert.True(valueTaskAwaiter.IsCompleted);
        valueTaskAwaiter.GetResult();

        var configuredValueTaskAwaiter = Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredValueTaskAwaiter.IsCompleted);
        configuredValueTaskAwaiter.GetResult();
    }

    [Fact]
    public void AwaiterOfCompletedSomeIsCompletedWithTheResult()
    {
        var taskAwaiter = Option.Some(Task.FromResult(10)).GetAwaiter();
        Assert.True(taskAwaiter.IsCompleted);
        FunctionalAssert.Some(10, taskAwaiter.GetResult());

        var configuredTaskAwaiter = Option.Some(Task.FromResult(10)).ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredTaskAwaiter.IsCompleted);
        FunctionalAssert.Some(10, configuredTaskAwaiter.GetResult());

        var valueTaskAwaiter = Option.Some(ValueTask.FromResult(10)).GetAwaiter();
        Assert.True(valueTaskAwaiter.IsCompleted);
        FunctionalAssert.Some(10, valueTaskAwaiter.GetResult());

        var configuredValueTaskAwaiter = Option.Some(ValueTask.FromResult(10)).ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredValueTaskAwaiter.IsCompleted);
        FunctionalAssert.Some(10, configuredValueTaskAwaiter.GetResult());
    }

    [Fact]
    public void AwaiterOfCompletedVoidSomeIsCompleted()
    {
        var taskAwaiter = Option.Some(Task.CompletedTask).GetAwaiter();
        Assert.True(taskAwaiter.IsCompleted);
        taskAwaiter.GetResult();

        var configuredTaskAwaiter = Option.Some(Task.CompletedTask).ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredTaskAwaiter.IsCompleted);
        configuredTaskAwaiter.GetResult();

        var valueTaskAwaiter = Option.Some(ValueTask.CompletedTask).GetAwaiter();
        Assert.True(valueTaskAwaiter.IsCompleted);
        valueTaskAwaiter.GetResult();

        var configuredValueTaskAwaiter = Option.Some(ValueTask.CompletedTask).ConfigureAwait(false).GetAwaiter();
        Assert.True(configuredValueTaskAwaiter.IsCompleted);
        configuredValueTaskAwaiter.GetResult();
    }
#pragma warning restore xUnit1031

    [Fact]
    public void OnCompletedInvokesTheContinuationWhenTheTaskCompletes()
    {
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(source.Task).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(source.Task).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some<Task>(source.Task).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some<Task>(source.Task).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
    }

    [Fact]
    public void UnsafeOnCompletedInvokesTheContinuationWhenTheTaskCompletes()
    {
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(source.Task).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(source.Task).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some<Task>(source.Task).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some<Task>(source.Task).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedOnCompletion<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void ContinuationForCompletedSomeIsPostedToTheCurrentSynchronizationContext()
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

    // The awaiters of Option<Task<T>> and Option<ValueTask<T>> hold an Option of the underlying awaiter and invoke the
    // continuation of None inline, whereas the awaiters of Option<Task> and Option<ValueTask> replace None by a completed
    // task, whose awaiter posts the continuation to the current synchronization context like any completed task does.
    [Fact]
    public void ContinuationForGenericNoneIsInvokedInline()
    {
        AssertContinuationIsInvokedInline(continuation => Option<Task<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<Task<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<Task<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask<int>>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask<int>>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask<int>>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<Task>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsInvokedInline(continuation => Option<ValueTask>.None.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void ContinuationForVoidNoneIsPostedToTheCurrentSynchronizationContext()
    {
        AssertContinuationIsPosted(continuation => Option<Task>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option<Task>.None.GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option<ValueTask>.None.GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsPosted(continuation => Option<ValueTask>.None.GetAwaiter().UnsafeOnCompleted(continuation));
    }

    [Fact]
    public void ConfigureAwaitFalseBypassesTheSynchronizationContextOfTheRegistration()
    {
        AssertContinuationIsNotPosted<int>((source, continuation) => Option.Some(source.Task).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted<int>((source, continuation) => Option.Some(source.Task).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted<int>((source, continuation) => Option.Some(new ValueTask<int>(source.Task)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted<Unit>((source, continuation) => Option.Some<Task>(source.Task).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted<Unit>((source, continuation) => Option.Some<Task>(source.Task).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
        AssertContinuationIsNotPosted<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).ConfigureAwait(false).GetAwaiter().OnCompleted(continuation));
        AssertContinuationIsNotPosted<Unit>((source, continuation) => Option.Some(new ValueTask(source.Task)).ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(continuation));
    }

    // None of the helpers below waits for a scheduler. A continuation registered on an already completed task is
    // handed to the current synchronization context, or to the thread pool when there is none, and both are shared
    // with every other test in the process: xunit's MaxConcurrencySyncContext has one worker per core and a loaded
    // two-core runner starved both of them for more than five seconds (#1004). The helpers therefore either complete
    // the task themselves on a thread without synchronization context, where the BCL runs the await continuation
    // inline within SetResult, or install a recording context whose Post runs the callback inline.
    private static void AssertContinuationIsInvokedOnCompletion<TResult>(Action<TaskCompletionSource<TResult>, Action> registerContinuation)
        => WithoutSynchronizationContext(() =>
        {
            var source = new TaskCompletionSource<TResult>();
            var invoked = false;
            registerContinuation(source, () => invoked = true);
            Assert.False(invoked, "the continuation was invoked before the task completed");
            source.SetResult(default!);
            Assert.True(invoked, "the continuation was not invoked when the task completed");
        });

    private static void AssertContinuationIsInvokedInline(Action<Action> registerContinuation)
        => WithSynchronizationContext(new RecordingSynchronizationContext(), context =>
        {
            var invoked = false;
            registerContinuation(() => invoked = true);
            Assert.True(invoked, "the continuation was not invoked");
            Assert.Equal(0, context.Posted);
        });

    private static void AssertContinuationIsPosted(Action<Action> registerContinuation)
        => WithSynchronizationContext(new RecordingSynchronizationContext(), context =>
        {
            var invoked = false;
            registerContinuation(() => invoked = true);
            Assert.Equal(1, context.Posted);
            Assert.True(invoked, "the continuation was not invoked");
        });

    // The context is only installed while registering: a continuation that did capture it would post to it when the
    // task completes, one that did not runs inline within SetResult because no context is installed by then.
    private static void AssertContinuationIsNotPosted<TResult>(Action<TaskCompletionSource<TResult>, Action> registerContinuation)
        => WithoutSynchronizationContext(() =>
        {
            var source = new TaskCompletionSource<TResult>();
            var invoked = false;
            var context = new RecordingSynchronizationContext();
            WithSynchronizationContext(context, _ => registerContinuation(source, () => invoked = true));
            source.SetResult(default!);
            Assert.True(invoked, "the continuation was not invoked when the task completed");
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
