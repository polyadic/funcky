namespace Funcky.Test.Extensions;

public sealed class ListExtensionsTest
{
    [Fact]
    public void GivenAListIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IList<string> list = new List<string> { "Alpha", "Gamma", "Epsilon" };

        FunctionalAssert.None(list.IndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IList<string> list = new List<string> { "Alpha", "Gamma", "Epsilon" };

        FunctionalAssert.Some(2, list.IndexOfOrNone("Epsilon"));
    }
}
