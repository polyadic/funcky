namespace Funcky.Test.Monads;

public sealed class OptionComparerTest
{
    [Fact]
    public void DefaultComparerIsCached()
    {
        Assert.Same(OptionComparer<int>.Default, OptionComparer<int>.Default);
        Assert.Same(OptionComparer<string>.Default, OptionComparer<string>.Default);
    }

    [Fact]
    public void NoneIsLessThanSomeAndSomeValuesUseTheItemComparer()
    {
        var comparer = OptionComparer<int>.Default;

        Assert.True(comparer.Compare(Option<int>.None, Option.Some(1)) < 0);
        Assert.True(comparer.Compare(Option.Some(1), Option<int>.None) > 0);
        Assert.Equal(0, comparer.Compare(Option<int>.None, Option<int>.None));
        Assert.True(comparer.Compare(Option.Some(1), Option.Some(2)) < 0);
        Assert.Equal(0, comparer.Compare(Option.Some(2), Option.Some(2)));
    }

    [Fact]
    public void CreateWithComparisonUsesTheComparison()
    {
        var reversed = OptionComparer<int>.Create((x, y) => y.CompareTo(x));

        Assert.True(reversed.Compare(Option.Some(1), Option.Some(2)) > 0);
        Assert.True(reversed.Compare(Option<int>.None, Option.Some(2)) < 0);
    }
}
