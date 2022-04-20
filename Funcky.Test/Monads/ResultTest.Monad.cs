using FsCheck;
using FsCheck.Xunit;
using Result = Funcky.Monads.Result;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    public ResultTest() => FunckyGenerators.Register();

    [Property]
    public Property AssociativityHolds(
        Result<int> input,
        Func<int, Result<int>> selectorOne,
        Func<int, Result<int>> selectorTwo)
    {
        Result<int> CombinedSelector(int x) => selectorOne(x).SelectMany(selectorTwo);

        return (input.SelectMany(selectorOne).SelectMany(selectorTwo) == input.SelectMany(CombinedSelector))
            .ToProperty();
    }

    [Property]
    public Property RightIdentityHolds(Result<int> input)
        => (input.SelectMany(Result.Return) == input).ToProperty();

    [Property]
    public Property LeftIdentityHolds(int input, Func<int, Result<int>> selector)
        => (Result.Return(input).SelectMany(selector) == selector(input)).ToProperty();
}
