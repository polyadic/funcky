namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ElementAtOrNoneTest
{
    [Fact]
    public void GivenAnEmptySequenceElementAtOrNoneAlwaysReturnsNone()
    {
        var empty = Enumerable.Empty<string>().ToList();

        foreach (var index in Enumerable.Range(-5, 5))
        {
            FunctionalAssert.None(empty.ElementAtOrNone(index));
        }
    }

    [Fact]
    public void GivenANonEmptySequenceElementAtOrNoneReturnsSomeIfItsInTheRageOtherwiseNone()
    {
        var range = Enumerable.Range(1, 5);

        FunctionalAssert.None(range.ElementAtOrNone(-42));
        FunctionalAssert.None(range.ElementAtOrNone(-1));
        FunctionalAssert.Some(1, range.ElementAtOrNone(0));
        FunctionalAssert.Some(2, range.ElementAtOrNone(1));
        FunctionalAssert.Some(3, range.ElementAtOrNone(2));
        FunctionalAssert.Some(4, range.ElementAtOrNone(3));
        FunctionalAssert.Some(5, range.ElementAtOrNone(4));
        FunctionalAssert.None(range.ElementAtOrNone(5));
        FunctionalAssert.None(range.ElementAtOrNone(42));
        FunctionalAssert.None(range.ElementAtOrNone(1337));
    }
}
