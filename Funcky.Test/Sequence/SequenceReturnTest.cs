using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Extensions;

namespace Funcky.Test
{
    public sealed class SequenceReturnTest
    {
        [Property]
        public Property ReturnOfASingleItemElevatesThatItemIntoASingleItemedEnumerable(int item)
        {
            var sequence = Sequence.Return(item);

            return (sequence.SingleOrNone() == item).ToProperty();
        }

        [Property]
        public Property SequenceReturnCreatesAnEnumerableFromAnArbitraryNumberOfParameters(string one, string two, string three)
        {
            var sequence = Sequence.Return(one, two, three);

            return Enumerable.SequenceEqual(new[] { one, two, three }, sequence).ToProperty();
        }
    }
}
