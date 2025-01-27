using FsCheck;
using Funcky.FsCheck;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    [FunckyProperty]
    public Property AssociativityHolds(
        Either<int, int> input,
        Func<int, Either<int, int>> selectorOne,
        Func<int, Either<int, int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHolds(Either<int, int> input)
        => CheckAssert.Equal(input, input.SelectMany(Either<int>.Return));

    [FunckyProperty]
    public Property LeftIdentityHolds(int input, Func<int, Either<int, int>> selector)
        => CheckAssert.Equal(Either<int>.Return(input).SelectMany(selector), selector(input));

    [FunckyProperty]
    public Property AssociativityHoldsWithReferenceTypes(
        Either<string, string> input,
        Func<string, Either<string, string>> selectorOne,
        Func<string, Either<string, string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)));

    [FunckyProperty]
    public Property RightIdentityHoldsWithReferenceTypes(Either<string, string> input)
        => CheckAssert.Equal(input, input.SelectMany(Either<string>.Return));

    [FunckyProperty]
    public Property LeftIdentityHoldsWithReferenceTypes(global::FsCheck.NonNull<string> input, Func<string, Either<string, string>> selector)
        => CheckAssert.Equal(Either<string>.Return(input.Get).SelectMany(selector), selector(input.Get));

    private static Func<TItem, Either<TItem, TItem>> Combine<TItem>(Func<TItem, Either<TItem, TItem>> functionA, Func<TItem, Either<TItem, TItem>> functionB)
        where TItem : notnull
        => input
            => functionA(input).SelectMany(functionB);
}
