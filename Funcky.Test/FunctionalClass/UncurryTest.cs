using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.FunctionalClass;

public sealed class UncurryTest
{
    [Property]
    public Property GivenASecondLevelCurriedFunctionWeGetAFunctionWith2Parameters(int number, string text)
    {
        Func<int, Func<string, string>> f = number => text => $"number:{number}, text:{text}";

        return (f(number)(text) == Uncurry(f)(number, text)).ToProperty();
    }

    [Property]
    public Property GivenA3RdLevelCurriedFunctionWeGetAFunctionWith3Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, string>>> f = number => text => p3 => $"number:{number}, text:{text}, {p3}";

        return (f(number)(text)(true) == Uncurry(f)(number, text, true)).ToProperty();
    }

    [Property]
    public Property GivenA4ThLevelCurriedFunctionWeGetAFunctionWith4Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, Func<bool, string>>>> f = number => text => p3 => p4 => $"number:{number}, text:{text}, {p3}, {p4}";

        return (f(number)(text)(true)(false) == Uncurry(f)(number, text, true, false)).ToProperty();
    }

    [Property]
    public Property GivenA5ThLevelCurriedFunctionWeGetAFunctionWith5Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, Func<bool, Func<bool, string>>>>> f = number => text => p3 => p4 => p5 => $"number:{number}, text:{text}, {p3}, {p4}, {p5}";

        return (f(number)(text)(true)(false)(true) == Uncurry(f)(number, text, true, false, true)).ToProperty();
    }

    [Property]
    public Property GivenA6ThLevelCurriedFunctionWeGetAFunctionWith6Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>> f = number => text => p3 => p4 => p5 => p6 => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

        return (f(number)(text)(true)(false)(true)(false) == Uncurry(f)(number, text, true, false, true, false)).ToProperty();
    }

    [Property]
    public Property GivenA7ThLevelCurriedFunctionWeGetAFunctionWith7Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>>> f = number => text => p3 => p4 => p5 => p6 => p7 => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

        return (f(number)(text)(true)(false)(true)(false)(true) == Uncurry(f)(number, text, true, false, true, false, true)).ToProperty();
    }

    [Property]
    public Property GivenA8ThLevelCurriedFunctionWeGetAFunctionWith8Parameters(int number, string text)
    {
        Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>>>> f = number => text => p3 => p4 => p5 => p6 => p7 => p8 => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}, {p8}";

        return (f(number)(text)(true)(false)(true)(false)(true)(false) == Uncurry(f)(number, text, true, false, true, false, true, false)).ToProperty();
    }
}
