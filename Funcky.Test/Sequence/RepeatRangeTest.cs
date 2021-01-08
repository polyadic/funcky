using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public class RepeatRangeTest
    {
        [Property]
        public Property TheLengthOfTheGeneratedRepeatRangeIsCorrect(List<int> list, NonNegativeInt count)
        {
            var sequence = Sequence
                .RepeatRange(list, count.Get)
                .ToList();

            return (sequence.Count == list.Count * count.Get).ToProperty();
        }

        [Property]
        public Property AllSubsequentSubListsWithOffsetModuloCountAreIdenticalToTheGivenList(List<int> list, NonNegativeInt count)
        {
            var sequence = Sequence
                .RepeatRange(list, count.Get)
                .ToList();

            return Enumerable
                .Range(0, count.Get)
                .Aggregate(true, (b, i)
                    => b && sequence
                    .Skip(i * list.Count)
                    .Zip(list, (l, r) => l == r)
                    .All(Identity)).ToProperty();
        }
    }
}
