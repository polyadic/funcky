using System.Collections.Immutable;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.ImmutableListExtensions;

public sealed class LastIndexOfOrNoneTest
{
    [Fact]
    public void GivenAListLastIndexOfOrNoneReturnsNoneIfTheElementIsNotInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.None(list.LastIndexOfOrNone("Beta"));
    }

    [Fact]
    public void GivenAListLastIndexOfOrNoneReturnsSomeIndexIfTheElementIsInTheList()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.Some(3, list.LastIndexOfOrNone("Alpha"));
    }

    [Fact]
    public void AllOverloadsOfLastIndexOfOrNoneGiveUsefulResult()
    {
        IImmutableList<string> list = ImmutableList<string>.Empty.Add("Alpha").Add("Gamma").Add("Epsilon").Add("Alpha").Add("Gamma");

        FunctionalAssert.Some(4, list.LastIndexOfOrNone("Gamma"));
        FunctionalAssert.Some(1, list.LastIndexOfOrNone("Gamma", 2));
        FunctionalAssert.None(list.LastIndexOfOrNone("Gamma", 3, 2));
        FunctionalAssert.Some(4, list.LastIndexOfOrNone("Gamma", new EverythingIsEqual<string>()));
        FunctionalAssert.Some(2, list.LastIndexOfOrNone("Gamma", 2, 1, new EverythingIsEqual<string>()));
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

        FunctionalAssert.None(list.LastIndexOfOrNone("Gamma", 0));
        FunctionalAssert.None(list.LastIndexOfOrNone("Gamma", 0, 0));
    }
}
