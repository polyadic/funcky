using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Test
{
    public sealed class RepeatRangeTest
    {
        [Property]
        public Property TheLengthOfTheGeneratedRepeatRangeIsCorrect(List<int> list, NonNegativeInt count)
        {
            var sequence = AsyncSequence
                .RepeatRange(list, count.Get)
                .ToListAsync().Result;

            return (sequence.Count == list.Count * count.Get).ToProperty();
        }

        [Property]
        public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
            => AsyncSequence
                .RepeatRange(list, count.Get)
                .IsSequenceRepeating(list)
                .NTimes(count.Get);
    }
}
