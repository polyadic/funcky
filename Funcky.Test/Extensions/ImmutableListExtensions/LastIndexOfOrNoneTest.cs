using System.Collections.Immutable;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.ImmutableListExtensions;

public sealed class LastIndexOfOrNoneTest
{
    [Fact]
    public void GivenAListLastIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.None(list.LastIndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListLastIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.Some(3, list.LastIndexOfOrNone("Alpha"));
    }

    [Fact]
    public void AllOverloadsOfLastIndexOfOrNoneGiveUsefulResult()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        Assert.Some(4, list.LastIndexOfOrNone("Gamma"));
        Assert.Some(1, list.LastIndexOfOrNone("Gamma", 2));
        Assert.None(list.LastIndexOfOrNone("Gamma", 3, 2));
        Assert.Some(4, list.LastIndexOfOrNone("Gamma", new EverythingIsEqual<string>()));
        Assert.Some(2, list.LastIndexOfOrNone("Gamma", 2, 1, new EverythingIsEqual<string>()));
    }

    [Fact]
    public void GivenIndexesOutsideTheListLastIndexOfOrNoneThrowsAnException()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Test");

        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 15));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 15, 7));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 15, 7, new EverythingIsEqual<string>()));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", -15));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOfOrNone("Gamma", 0, -2));
    }

    [Fact]
    public void GivenZeroIndexLastIndexOfOrNoneReturnsNone()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty;

        Assert.None(list.LastIndexOfOrNone("Gamma", 0));
        Assert.None(list.LastIndexOfOrNone("Gamma", 0, 0));
    }
}
