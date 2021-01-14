using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test
{
    public sealed class RepeatRangeTest
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
        public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
            => Sequence
                .RepeatRange(list, count.Get)
                .IsSequenceRepeating(list)
                .NTimes(count.Get);
    }
}
