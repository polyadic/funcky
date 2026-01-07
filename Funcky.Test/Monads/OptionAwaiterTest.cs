#if !NET48

namespace Funcky.Test.Monads;

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
}

#endif
