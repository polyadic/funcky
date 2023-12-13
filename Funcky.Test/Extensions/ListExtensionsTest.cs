#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
namespace Funcky.Test.Extensions;

public sealed class ListExtensionsTest
{
    [Fact]
    public void GivenAListIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IList<string> list = ["Alpha", "Gamma", "Epsilon"];

        FunctionalAssert.None(list.IndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IList<string> list = ["Alpha", "Gamma", "Epsilon"];

        FunctionalAssert.Some(2, list.IndexOfOrNone("Epsilon"));
    }
}
