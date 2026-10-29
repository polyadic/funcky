namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ElementAtOrNoneTest
{
    [Fact]
    public void GivenAnEmptySequenceElementAtOrNoneAlwaysReturnsNone()
    {
        var empty = Enumerable.Empty<string>().ToList();

        foreach (var index in Enumerable.Range(-5, 5))
        {
            Assert.None(empty.ElementAtOrNone(index));
        }
    }

    [Fact]
    public void GivenANonEmptySequenceElementAtOrNoneReturnsSomeIfItsInTheRageOtherwiseNone()
    {
        var range = Enumerable.Range(1, 5);

        Assert.None(range.ElementAtOrNone(-42));
        Assert.None(range.ElementAtOrNone(-1));
        Assert.Some(1, range.ElementAtOrNone(0));
        Assert.Some(2, range.ElementAtOrNone(1));
        Assert.Some(3, range.ElementAtOrNone(2));
        Assert.Some(4, range.ElementAtOrNone(3));
        Assert.Some(5, range.ElementAtOrNone(4));
        Assert.None(range.ElementAtOrNone(5));
        Assert.None(range.ElementAtOrNone(42));
        Assert.None(range.ElementAtOrNone(1337));
    }
}
