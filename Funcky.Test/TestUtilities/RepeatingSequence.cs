namespace Funcky.Test.TestUtilities;

public static class RepeatingSequence
{
    public static RepeatingSequenceHelper IsSequenceRepeating(this IEnumerable<int> sequence, IEnumerable<int> pattern)
        => new(sequence, pattern);

#if INTEGRATED_ASYNC
    public static RepeatingAsyncSequenceHelper IsSequenceRepeating(this IAsyncEnumerable<int> sequence, IAsyncEnumerable<int> pattern)
        => new(sequence, pattern);
#endif

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

#if INTEGRATED_ASYNC
    public class RepeatingAsyncSequenceHelper
    {
        private readonly IAsyncEnumerable<int> _sequence;
        private readonly IAsyncEnumerable<int> _pattern;

        internal RepeatingAsyncSequenceHelper(IAsyncEnumerable<int> sequence, IAsyncEnumerable<int> pattern)
            => (_sequence, _pattern) = (sequence, pattern);

        public async Task<bool> NTimes(int count)
            => await AsyncEnumerable
                .Range(0, count)
                .AggregateAsync(true, AggregateEquality);

        public async ValueTask<bool> AggregateEquality(bool b, int i, CancellationToken token)
            => b && await _sequence
                .Skip(i * await _pattern.CountAsync(token))
                .Zip(_pattern, (l, r) => l == r)
                .AllAsync(Identity, token);
    }
#endif
}
