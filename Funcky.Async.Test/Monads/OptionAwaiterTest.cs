namespace Funcky.Async.Test.Monads;

public sealed class OptionAwaiterTest
{
    [Fact]
    public async Task OptionOfVoidTaskIsAwaitable()
    {
        await Option.Some(Task.Delay(10));
        await Option<Task>.None;
    }

    [Fact]
    public async Task OptionOfTaskIsAwaitable()
    {
        FunctionalAssert.Some(10, await Option.Some(Task.FromResult(10)));
        FunctionalAssert.None(await Option<Task<int>>.None);
    }

    [Fact]
    public async Task OptionOfVoidValueTaskIsAwaitable()
    {
        await Option.Some(new ValueTask(Task.Delay(10)));
        await Option<ValueTask>.None;
    }

    [Fact]
    public async Task OptionOfValueTaskIsAwaitable()
    {
        FunctionalAssert.Some(10, await Option.Some(ValueTask.FromResult(10)));
        FunctionalAssert.None(await Option<ValueTask<int>>.None);
    }
}
