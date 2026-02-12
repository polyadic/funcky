using Funcky.Test.TestUtilities;
using static Funcky.Discard;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class WhereSelectTest
{
    [Fact]
    public void WhereSelectNullIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.WhereSelect(_ => Option<object>.None);
    }

    [Fact]
    public void WhereSelectFiltersEmptySelectorValues()
    {
        var input = Enumerable.Range(0, 10);
        var expectedResult = new[] { 0, 4, 16, 36, 64 };
        var result = input.WhereSelect(SquareEvenNumbers);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void WhereSelectReceivesTheSourceElementsIndex()
    {
        const int count = 6;
        var expectedSequence = Enumerable.Range(0, count: count);
        var units = Sequence.Cycle(__).Take(count);
        var indexes = units.WhereSelect((_, index) => Option.Some(index));
        Assert.Equal(expectedSequence, indexes);
    }

    [Fact]
    public void WhereSelectFiltersEmptyFromSequence()
    {
        var withNone = new List<Option<int>> { Option<int>.None, 0, Option<int>.None, 1337, Option<int>.None, 42, 0, 12, Option<int>.None, 1 };
        var expectedResult = new List<int> { 0, 1337, 42, 0, 12, 1 };

        Assert.Equal(expectedResult, withNone.WhereSelect());
    }

    private static Option<int> SquareEvenNumbers(int number)
        => Option.FromBoolean(IsEven(number), number * number);

    private static bool IsEven(int number)
        => number % 2 == 0;
}
