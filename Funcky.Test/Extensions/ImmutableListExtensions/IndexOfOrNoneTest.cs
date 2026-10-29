using System.Collections.Immutable;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.ImmutableListExtensions;

public sealed class IndexOfOrNoneTest
{
    [Fact]
    public void GivenAListIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.None(list.IndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.Some(0, list.IndexOfOrNone("Alpha"));
    }

    [Fact]
    public void AllOverloadsOfIndexOfOrNoneGiveUsefulResult()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.Some(1, list.IndexOfOrNone("Gamma"));
        Assert.Some(4, list.IndexOfOrNone("Gamma", 2));
        Assert.None(list.IndexOfOrNone("Gamma", 2, 2));
        Assert.Some(0, list.IndexOfOrNone("Gamma", new EverythingIsEqual<string>()));
        Assert.Some(2, list.IndexOfOrNone("Gamma", 2, 1, new EverythingIsEqual<string>()));
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

        Assert.None(list.IndexOfOrNone("Gamma", 0));
        Assert.None(list.IndexOfOrNone("Gamma", 0, 0));
    }

    [Fact]
    public void CallIsNotAmbiguousWhenUsedOnConcreteTypes()
    {
        _ = ImmutableArray<string>.Empty.IndexOfOrNone("foo");
        _ = ImmutableList<string>.Empty.IndexOfOrNone("foo");
    }
}
