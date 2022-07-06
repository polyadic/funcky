using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Property]
    public Property AssociativityHolds(
        Option<int> input,
        Func<int, Option<int>> selectorOne,
        Func<int, Option<int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [Property]
    public Property RightIdentityHolds(Option<int> input)
        => CheckAssert.Equal(input.SelectMany(Option.Some), input);

    [Property]
    public Property LeftIdentityHolds(int input, Func<int, Option<int>> function)
        => CheckAssert.Equal(Option.Some(input).SelectMany(function), function(input));

    [Property]
    public Property AssociativityHoldsWithReferenceTypes(
        Option<string> input,
        Func<string, Option<string>> selectorOne,
        Func<string, Option<string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [Property]
    public Property RightIdentityHoldsWithReferenceTypes(Option<string> option)
        => CheckAssert.Equal(option.SelectMany(Option.Some), option);

    [Property]
    public Property LeftIdentityHoldsWithReferenceTypes(string? input, Func<string, Option<string>> function)
        => input is null
            ? true.ToProperty()
            : CheckAssert.Equal(Option.FromNullable(input).SelectMany(function), function(input));

    private static Func<TItem, Option<TItem>> Combine<TItem>(Func<TItem, Option<TItem>> functionA, Func<TItem, Option<TItem>> functionB)
        where TItem : notnull
        => input
            => functionA(input).SelectMany(functionB);
}
