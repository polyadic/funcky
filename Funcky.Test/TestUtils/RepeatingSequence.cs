using FsCheck;

namespace Funcky.Test.TestUtils;

public static class RepeatingSequence
{
    public static RepeatingSequenceHelper IsSequenceRepeating(this IEnumerable<int> sequence, IEnumerable<int> pattern)
        => new(sequence, pattern);

    public class RepeatingSequenceHelper
    {
        private readonly IEnumerable<int> _sequence;
        private readonly IEnumerable<int> _pattern;

        internal RepeatingSequenceHelper(IEnumerable<int> sequence, IEnumerable<int> pattern)
        {
            _sequence = sequence;
            _pattern = pattern;
        }

        public bool NTimes(int count)
            => Enumerable
                .Range(0, count)
                .Aggregate(true, AggregateEquality);

        public bool AggregateEquality(bool b, int i)
            => b && _sequence
                .Skip(i * _pattern.Count())
                .Zip(_pattern, (l, r) => l == r)
                .All(Identity);
    }
}
