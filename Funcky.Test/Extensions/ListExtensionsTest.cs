#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
namespace Funcky.Test.Extensions;

public sealed class ListExtensionsTest
{
    [Fact]
    public void GivenAListIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IList<string> list = ["Alpha", "Gamma", "Epsilon"];

        Assert.None(list.IndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IList<string> list = ["Alpha", "Gamma", "Epsilon"];

        Assert.Some(2, list.IndexOfOrNone("Epsilon"));
    }

    [Fact]
    public void GivenAListAndAPredicateFindIndexOrNoneReturnsTheFirstElementMatchingThePredicateInTheGivenInterval()
    {
        List<string> list = ["Alpha", "Gamma", "Alien", "Epsilon"];

        Assert.Some(0, list.FindIndexOrNone(item => item.StartsWith("Al")));
        Assert.Some(2, list.FindIndexOrNone(2, item => item.StartsWith("Al")));
        Assert.None(list.FindIndexOrNone(3, 1, item => item.StartsWith("Al")));
    }

    [Fact]
    public void GivenAListAndAPredicateFindLastIndexOrNoneReturnsTheLastElementMatchingThePredicateInTheGivenInterval()
    {
        List<string> list = ["Alpha", "Gamma", "Alien", "Epsilon"];

        Assert.Some(2, list.FindLastIndexOrNone(item => item.StartsWith("Al")));
        Assert.Some(0, list.FindLastIndexOrNone(1, item => item.StartsWith("Al")));
        Assert.None(list.FindLastIndexOrNone(3, 1, item => item.StartsWith("Al")));
    }
}
