namespace Funcky.Test.TestUtils;

public static class RepeatingSequence
{
    public static RepeatingSequenceHelper IsSequenceRepeating(this IEnumerable<int> sequence, IEnumerable<int> pattern)
        => new(sequence, pattern);

    public class RepeatingSequenceHelper(IEnumerable<int> sequence, IEnumerable<int> pattern)
    {
        public bool NTimes(int count)
            => Enumerable
                .Range(0, count)
                .Aggregate(true, AggregateEquality);

        public bool AggregateEquality(bool b, int i)
            => b && sequence
                .Skip(i * pattern.Count())
                .Zip(pattern, (l, r) => l == r)
                .All(Identity);
    }
}
