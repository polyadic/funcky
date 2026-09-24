#if GENERIC_MATH
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions;

public sealed class NumberExtensionsTest
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(-1, false)]
    [InlineData(11, false)]
    public void InRangeWithInclusiveBoundariesContainsBothEnds(int number, bool expected)
        => Assert.Equal(expected, number.InRange<Including, Including>(0, 10));

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    public void InRangeWithExclusiveBoundariesContainsNeitherEnd(int number, bool expected)
        => Assert.Equal(expected, number.InRange<Excluding, Excluding>(0, 10));

    [Theory]
    [InlineData(0, true)]
    [InlineData(10, false)]
    public void InRangeWithMixedBoundariesContainsOnlyTheInclusiveEnd(int number, bool expected)
    {
        Assert.Equal(expected, number.InRange<Including, Excluding>(0, 10));
        Assert.Equal(!expected, number.InRange<Excluding, Including>(0, 10));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(-1, false)]
    [InlineData(11, false)]
    public void InRangeAcceptsBoundariesInDescendingOrder(int number, bool expected)
    {
        Assert.Equal(expected, number.InRange<Including, Including>(10, 0));
        Assert.Equal(expected && number != 10, number.InRange<Excluding, Including>(10, 0));
        Assert.Equal(expected && number != 0, number.InRange<Including, Excluding>(10, 0));
    }

    [Fact]
    public void InRangeWithEqualBoundariesOnlyContainsTheBoundaryWhenBothAreInclusive()
    {
        Assert.True(5.InRange<Including, Including>(5, 5));
        Assert.False(5.InRange<Including, Excluding>(5, 5));
        Assert.False(5.InRange<Excluding, Including>(5, 5));
        Assert.False(5.InRange<Excluding, Excluding>(5, 5));
    }

    [Fact]
    public void InRangeWorksForAnyTypeSupportingComparisonOperators()
    {
        Assert.True(0.5.InRange<Including, Excluding>(0.0, 1.0));
        Assert.False(1.0.InRange<Including, Excluding>(0.0, 1.0));
        Assert.True(1.0m.InRange<Excluding, Including>(0.0m, 1.0m));
        Assert.True(((Int128)1).InRange<Including, Excluding>(Int128.Zero, Int128.MaxValue));
        Assert.True(System.Numerics.BigInteger.One.InRange<Including, Excluding>(System.Numerics.BigInteger.Zero, System.Numerics.BigInteger.Pow(10, 40)));
        Assert.True('b'.InRange<Including, Including>('a', 'z'));
        Assert.True(((Half)0.5).InRange<Excluding, Excluding>((Half)0, (Half)1));
    }

    [Property]
    public Property InRangeWithInclusiveBoundariesMatchesMinAndMax(int number, int first, int second)
        => (number.InRange<Including, Including>(first, second) == (Math.Min(first, second) <= number && number <= Math.Max(first, second))).ToProperty();

    [Property]
    public Property InRangeWithExclusiveBoundariesMatchesMinAndMax(int number, int first, int second)
        => (number.InRange<Excluding, Excluding>(first, second) == (Math.Min(first, second) < number && number < Math.Max(first, second))).ToProperty();
}
#endif
