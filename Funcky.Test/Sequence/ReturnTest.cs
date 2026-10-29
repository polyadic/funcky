#pragma warning disable λ1202 // This test exercises the classic Sequence form itself.

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test;

public sealed class ReturnTest
{
    [Property]
    public Property ReturnOfASingleItemElevatesThatItemIntoASingleItemedEnumerable(int item)
    {
        var sequence = Sequence.Return(item);

        return (sequence.SingleOrNone() == item).ToProperty();
    }

    [Fact]
    public void SequenceReturnCreatesAnEnumerableFromAnArbitraryNumberOfParameters()
    {
        const string one = "Alpha";
        const string two = "Beta";
        const string three = "Gamma";

        var sequence = Sequence.Return(one, two, three);

        Assert.Collection(
            sequence,
            element1 => Assert.Equal(one, element1),
            element2 => Assert.Equal(two, element2),
            element3 => Assert.Equal(three, element3));
    }

    [Fact]
    public void TheSequenceCannotBeCastBackToAMutableArray()
    {
        var sequence = Sequence.Return(1, 2, 3);

        Assert.IsNotType<int[]>(sequence);
        var list = Assert.IsAssignableFrom<IList<int>>(sequence);
        Assert.True(list.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => list[0] = 42);
    }

    [Fact]
    public void TheSequenceIsAReadOnlyViewOverTheArrayPassedIn()
    {
        var array = new[] { 1, 2, 3 };
        var sequence = Sequence.Return(array);

        array[0] = 42;

        Assert.Equal([42, 2, 3], sequence);
        Assert.Equal(3, sequence.Count);
    }

    [Fact]
    public void PassingAnArrayBindsToTheParamsOverload()
    {
        var array = new[] { 1, 2, 3 };

        Assert.Equal(3, Sequence.Return(array).Count);
        Assert.Single(Sequence.Return<int[]>(array));
    }
}
