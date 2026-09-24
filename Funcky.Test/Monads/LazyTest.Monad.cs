using FsCheck;
using Funcky.FsCheck;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Monads;

public sealed partial class LazyTest
{
    [FunckyProperty]
    public Property AssociativityHolds(
        Lazy<int> input,
        Func<int, Lazy<int>> selectorOne,
        Func<int, Lazy<int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHolds(Lazy<int> input)
        => CheckAssert.Equal(input.SelectMany(Lazy.Return), input);

    [FunckyProperty]
    public Property LeftIdentityHolds(int input, Func<int, Lazy<int>> function)
        => CheckAssert.Equal(Lazy.Return(input).SelectMany(function), function(input));

    [FunckyProperty]
    public Property AssociativityHoldsWithReferenceTypes(
        Lazy<string> input,
        Func<string, Lazy<string>> selectorOne,
        Func<string, Lazy<string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHoldsWithReferenceTypes(Lazy<string> input)
        => CheckAssert.Equal(input.SelectMany(Lazy.Return), input);

    [FunckyProperty]
    public Property LeftIdentityHoldsWithReferenceTypes(string input, Func<string, Lazy<string>> function)
        => CheckAssert.Equal(Lazy.Return(input).SelectMany(function), function(input));

    private static Func<TItem, Lazy<TItem>> Combine<TItem>(Func<TItem, Lazy<TItem>> functionA, Func<TItem, Lazy<TItem>> functionB)
        where TItem : notnull
        => input
            => functionA(input).SelectMany(functionB);
}
