using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;
using Result = Funcky.Monads.Result;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Property]
    public Property AssociativityHolds(
        Result<int> input,
        Func<int, Result<int>> selectorOne,
        Func<int, Result<int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [Property]
    public Property RightIdentityHolds(Result<int> input)
        => CheckAssert.Equal(input, input.SelectMany(Result.Return));

    [Property]
    public Property LeftIdentityHolds(int input, Func<int, Result<int>> selector)
        => CheckAssert.Equal(Result.Return(input).SelectMany(selector), selector(input));

    [Property]
    public Property AssociativityHoldsWithReferenceTypes(
        Result<string> input,
        Func<string, Result<string>> selectorOne,
        Func<string, Result<string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [Property]
    public Property RightIdentityHoldsWithReferenceTypes(Result<string> input)
        => CheckAssert.Equal(input, input.SelectMany(Result.Return));

    [Property]
    public Property LeftIdentityHoldsWithReferenceTypes(string? input, Func<string, Result<string>> selector)
        => input is null
            ? true.ToProperty()
            : CheckAssert.Equal(Result.Return(input).SelectMany(selector), selector(input));

    private static Func<TItem, Result<TItem>> Combine<TItem>(Func<TItem, Result<TItem>> functionA, Func<TItem, Result<TItem>> functionB)
        => input
            => functionA(input).SelectMany(functionB);
}
