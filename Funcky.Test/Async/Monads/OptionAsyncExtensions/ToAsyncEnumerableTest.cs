namespace Funcky.Test.Async.Monads.OptionAsyncExtensions;

public sealed class ToAsyncEnumerableTest
{
    [Fact]
    public async Task ReturnsEmptyEnumerableWhenOptionIsEmpty()
    {
        var option = Option<int>.None;
        Assert.True(await option.ToAsyncEnumerable().NoneAsync());
    }

    [Fact]
    public async Task ReturnsEnumerableWithOneElementWhenOptionHasValue()
    {
        const int value = 42;
        var option = Option.Some(value);
        Assert.Equal(value, await option.ToAsyncEnumerable().SingleAsync());
    }
}
