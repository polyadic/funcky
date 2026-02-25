using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;
using Funcky.Test.TestUtilities;
using Result = Funcky.Monads.Result;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [FunckyProperty]
    public Property AssociativityHolds(
        Result<int> input,
        Func<int, Result<int>> selectorOne,
        Func<int, Result<int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHolds(Result<int> input)
        => CheckAssert.Equal(input, input.SelectMany(Result.Return));

    [FunckyProperty]
    public Property LeftIdentityHolds(int input, Func<int, Result<int>> selector)
        => CheckAssert.Equal(Result.Return(input).SelectMany(selector), selector(input));

    [FunckyProperty]
    public Property AssociativityHoldsWithReferenceTypes(
        Result<string> input,
        Func<string, Result<string>> selectorOne,
        Func<string, Result<string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHoldsWithReferenceTypes(Result<string> input)
        => CheckAssert.Equal(input, input.SelectMany(Result.Return));

    [FunckyProperty]
    public Property LeftIdentityHoldsWithReferenceTypes(string? input, Func<string, Result<string>> selector)
        => input is null
            ? true.ToProperty()
            : CheckAssert.Equal(Result.Return(input).SelectMany(selector), selector(input));

    private static Func<TItem, Result<TItem>> Combine<TItem>(Func<TItem, Result<TItem>> functionA, Func<TItem, Result<TItem>> functionB)
        where TItem : notnull
        => input
            => functionA(input).SelectMany(functionB);
}
