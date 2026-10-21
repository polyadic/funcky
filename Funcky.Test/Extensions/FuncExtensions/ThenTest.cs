using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.FuncExtensions;

public sealed class ThenTest
{
    [Property]
    public Property ThenIsComposeWithTheArgumentsSwapped(Func<string, int> f, Func<int, bool> g, string s)
        => (f.Then(g)(s) == g.Compose(f)(s)).ToProperty();

    [Property]
    public Property ThenAppliesTheReceiverFirst(int argument)
    {
        Func<int, int> addTwo = x => x + 2;
        Func<int, int> timesThree = x => x * 3;

        return (addTwo.Then(timesThree)(argument) == (argument + 2) * 3).ToProperty();
    }

    [Fact]
    public void ThenWorksWithANullaryFunction()
    {
        Func<int> nine = () => 9;
        Func<int, string> describe = x => $"{x} + 2";

        Assert.Equal("9 + 2", nine.Then(describe)());
    }

    [Property]
    public Property ThenWorksWithAnAction(int argument)
    {
        var sideEffect = new SideEffect();
        Func<int, string> describe = x => $"{x} + 2";
        Action<string> store = sideEffect.Store;

        describe.Then(store)(argument);

        return (sideEffect.Retrieve() == $"{argument} + 2").ToProperty();
    }

    [Fact]
    public void ThenWorksWithANullaryFunctionAndAnAction()
    {
        var sideEffect = new SideEffect();
        Func<string> greeting = () => "Hello World!";
        Action<string> store = sideEffect.Store;

        greeting.Then(store)();

        Assert.Equal("Hello World!", sideEffect.Retrieve());
    }

    [Fact]
    public void ThenChainsInEvaluationOrder()
    {
        Func<string, string> trim = s => s.Trim();
        Func<string, string> toLower = s => s.ToLowerInvariant();
        Func<string, Option<int>> parse = Funcky.Extensions.ParseExtensions.ParseInt32OrNone;

        var parseLoosely = trim.Then(toLower).Then(parse);

        FunctionalAssert.Some(42, parseLoosely(" 42 "));
        FunctionalAssert.None(parseLoosely("x"));
    }
}
