using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test
{
    public sealed class RepeatRangeTest
    {
        [Fact]
        public void RepeatRangeIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            using var repeatRange = Sequence.RepeatRange(doNotEnumerate, 2);
        }

        public Property TheLengthOfTheGeneratedRepeatRangeIsCorrect(List<int> list, NonNegativeInt count)
        {
            using var repeatRange = Sequence.RepeatRange(list, count.Get);

            var materialized = repeatRange.ToList();

            return (materialized.Count == list.Count * count.Get).ToProperty();
        }

        [Property]
        public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
        {
            using var repeatRange = Sequence.RepeatRange(list, count.Get);

            return repeatRange
                .IsSequenceRepeating(list)
                .NTimes(count.Get);
        }

        [Property]
        public void RepeatRangeEnumeratesUnderlyingEnumerableOnlyOnce(NonEmptySet<int> sequence)
        {
            var enumerateOnce = new EnumerateOnce<int>(sequence.Get);

            using var repeatRange = Sequence.RepeatRange(enumerateOnce, 3);

            repeatRange.ForEach(NoOperation);
        }
    }
}
