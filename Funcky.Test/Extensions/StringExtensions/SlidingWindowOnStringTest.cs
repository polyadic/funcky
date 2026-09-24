using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.StringExtensions;

public class SlidingWindowOnStringTest
{
    [Property]
    public Property ASlidingWindowFromAnEmptyStringWillAlwaysYieldAnEmptySequence(PositiveInt width)
        => string.Empty
            .SlidingWindow(width.Get)
            .None()
            .ToProperty();

    [Fact]
    public void SlidingWindowReturnsAListOfOverlappingPartialStrings()
    {
        const int width = 4;
        var source = "epsilon";

        Assert.Collection(
            source.SlidingWindow(width),
            window => Assert.Equal("epsi", window),
            window => Assert.Equal("psil", window),
            window => Assert.Equal("silo", window),
            window => Assert.Equal("ilon", window));
    }

    [Fact]
    public void GivenASourceStringEqualInLengthToTheSlidingWindowWidthReturnsASequenceWithOneElement()
    {
        var source = "gamma";

        Assert.Single(source.SlidingWindow(5));
        Assert.Equal(source, source.SlidingWindow(5).Single());
    }

    [Fact]
    public void GivenASourceSequenceShorterThanTheSlidingWindowWidthReturnsAnEmptySequence()
    {
        var source = "gamma";

        Assert.Empty(source.SlidingWindow(10));
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-42)]
    [InlineData(-1)]
    [InlineData(0)]
    public void SlidingWindowThrowsOnNonPositiveWidth(int width)
    {
        var source = "Just a simple test!";

        Assert.Throws<ArgumentOutOfRangeException>(() => source.SlidingWindow(width));
    }
}
