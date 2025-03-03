using System.Collections.Immutable;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.ImmutableListExtensions;

public sealed class IndexOfOrNoneTest
{
    [Fact]
    public void GivenAListIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.None(list.IndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.Some(0, list.IndexOfOrNone("Alpha"));
    }

    [Fact]
    public void AllOverloadsOfIndexOfOrNoneGiveUsefulResult()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.Some(1, list.IndexOfOrNone("Gamma"));
        FunctionalAssert.Some(4, list.IndexOfOrNone("Gamma", 2));
        FunctionalAssert.None(list.IndexOfOrNone("Gamma", 2, 2));
        FunctionalAssert.Some(0, list.IndexOfOrNone("Gamma", new EverythingIsEqual<string>()));
        FunctionalAssert.Some(2, list.IndexOfOrNone("Gamma", 2, 1, new EverythingIsEqual<string>()));
    }

    [Fact]
    public void GivenIndexesOutsideTheListIndexOfOrNoneThrowsAnException()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Test");

        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 15));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 15, 7));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 15, 7, new EverythingIsEqual<string>()));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", -15));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOfOrNone("Gamma", 0, -2));
    }

    [Fact]
    public void GivenZeroIndexIndexOfOrNoneReturnsNone()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty;

        FunctionalAssert.None(list.IndexOfOrNone("Gamma", 0));
        FunctionalAssert.None(list.IndexOfOrNone("Gamma", 0, 0));
    }

    [Fact]
    public void CallIsNotAmbiguousWhenUsedOnConcreteTypes()
    {
        _ = ImmutableArray<string>.Empty.IndexOfOrNone("foo");
        _ = ImmutableList<string>.Empty.IndexOfOrNone("foo");
    }
}
