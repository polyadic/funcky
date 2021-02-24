using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test
{
    public sealed class RepeatTest
    {
        [Property]
        public Property RepeatCallsGeneratorExactlyNumberTimes(int number)
        {
            var counter = 0;

            Sequence.Repeat(number, () => ++counter);

            return (number == counter)
                .ToProperty();
        }
    }
}
