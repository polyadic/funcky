using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;

namespace Funcky.Async.Test;

public sealed class ReturnTest
{
    [Property]
    public Property ReturnOfASingleItemElevatesThatItemIntoASingleItemedEnumerable(int item)
    {
        var sequence = AsyncSequence.Return(item);

        return (sequence.SingleOrNoneAsync().Result == item).ToProperty();
    }

    [Property]
    public Property SequenceReturnCreatesAnEnumerableFromAnArbitraryNumberOfParameters(string one, string two, string three)
    {
        var sequence = Sequence.Return(one, two, three);

        return Enumerable.SequenceEqual(new[] { one, two, three }, sequence).ToProperty();
    }
}
