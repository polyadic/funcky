namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ElementAtOrNoneTest
{
    [Fact]
    public void GivenAnEmptySequenceElementAtOrNoneAlwaysReturnsNone()
    {
        var empty = Enumerable.Empty<string>().ToList();

        foreach (var index in Enumerable.Range(-5, 5))
        {
            FunctionalAssert.IsNone(empty.ElementAtOrNone(index));
        }
    }

    [Fact]
    public void GivenANonEmptySequenceElementAtOrNoneReturnsSomeIfItsInTheRageOtherwiseNone()
    {
        var range = Enumerable.Range(1, 5);

        FunctionalAssert.IsNone(range.ElementAtOrNone(-42));
        FunctionalAssert.IsNone(range.ElementAtOrNone(-1));
        Assert.Equal(1, FunctionalAssert.IsSome(range.ElementAtOrNone(0)));
        Assert.Equal(2, FunctionalAssert.IsSome(range.ElementAtOrNone(1)));
        Assert.Equal(3, FunctionalAssert.IsSome(range.ElementAtOrNone(2)));
        Assert.Equal(4, FunctionalAssert.IsSome(range.ElementAtOrNone(3)));
        Assert.Equal(5, FunctionalAssert.IsSome(range.ElementAtOrNone(4)));
        FunctionalAssert.IsNone(range.ElementAtOrNone(5));
        FunctionalAssert.IsNone(range.ElementAtOrNone(42));
        FunctionalAssert.IsNone(range.ElementAtOrNone(1337));
    }
}
