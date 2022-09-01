using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed partial class ReaderTest
{
    [Property]
    public Property AssociativityHolds(
        int environment,
        Reader<int, int> input,
        Func<int, Reader<int, int>> selectorOne,
        Func<int, Reader<int, int>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)), environment);

    [Property]
    public Property RightIdentityHolds(int environment, Reader<int, int> input)
        => CheckAssert.Equal(input, input.SelectMany(Reader<int>.Return), environment);

    [Property]
    public Property LeftIdentityHoldsWithReferenceTypes(int environment, int input, Func<int, Reader<int, int>> selector)
        => CheckAssert.Equal(Reader<int>.Return(input).SelectMany(selector), selector(input), environment);

    [Property]
    public Property AssociativityHoldsWithReferenceTypes(
        string? environment,
        Reader<string, string> input,
        Func<string, Reader<string, string>> selectorOne,
        Func<string, Reader<string, string>> selectorTwo)
        => CheckAssert.Equal(input.SelectMany(selectorOne).SelectMany(selectorTwo), input.SelectMany(Combine(selectorOne, selectorTwo)), environment ?? string.Empty);

    [Property]
    public Property RightIdentityHoldsWithReferenceTypes(string environment, Reader<string, string> input)
        => CheckAssert.Equal(input, input.SelectMany(Reader<string>.Return), environment);

    [Property]
    public Property LeftIdentityHoldsWithReferenceTypesWithReferenceTypes(string environment, string input, Func<string, Reader<string, string>> selector)
        => CheckAssert.Equal(Reader<string>.Return(input).SelectMany(selector), selector(input), environment);

    private static Func<TItem, Reader<TItem, TItem>> Combine<TItem>(Func<TItem, Reader<TItem, TItem>> functionA, Func<TItem, Reader<TItem, TItem>> functionB)
        where TItem : notnull
        => input
            => functionA(input).SelectMany(functionB);
}
