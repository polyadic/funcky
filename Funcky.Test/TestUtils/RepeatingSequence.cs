using FsCheck;

namespace Funcky.Test.TestUtils
{
    public static class RepeatingSequence
    {
        public static RepeatingSequenceHelper IsSequenceRepeating(this IEnumerable<int> sequence, IEnumerable<int> pattern)
            => new(sequence, pattern);

        public class RepeatingSequenceHelper
        {
            private readonly IEnumerable<int> _sequence;
            private readonly IEnumerable<int> _pattern;

            internal RepeatingSequenceHelper(IEnumerable<int> sequence, IEnumerable<int> pattern)
                => (_sequence, _pattern) = (sequence, pattern);

            public Property NTimes(int count)
                => Enumerable
                    .Range(0, count)
                    .Aggregate(true, IsOneRepetition)
                    .ToProperty();

            private bool IsOneRepetition(bool isRepeating, int count)
                => isRepeating && _sequence
                    .Skip(count * _pattern.Count())
                    .Zip(_pattern, (l, r) => l == r)
                    .All(Identity);
        }
    }
}
