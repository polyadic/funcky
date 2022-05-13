using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test;

public sealed class ReturnTest
{
    [Property]
    public Property ReturnOfASingleItemElevatesThatItemIntoASingleItemedEnumerable(int item)
    {
        var sequence = AsyncSequence.Return(item);

        return (sequence.SingleOrNoneAsync().Result == item).ToProperty();
    }

    [Fact]
    public async Task SequenceReturnCreatesAnEnumerableFromAnArbitraryNumberOfParameters()
    {
        const string one = "Alpha";
        const string two = "Beta";
        const string three = "Gamma";

        var sequence = AsyncSequence.Return(one, two, three);

        await AsyncAssert.Collection(
            sequence,
            element1 => Assert.Equal(one, element1),
            element2 => Assert.Equal(two, element2),
            element3 => Assert.Equal(three, element3));
    }
}
