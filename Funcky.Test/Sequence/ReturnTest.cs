using FsCheck;
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
}
