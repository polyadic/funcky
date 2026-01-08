namespace Funcky.Test.Async.TestUtilities;

public static class RepeatingSequence
{
    public static RepeatingSequenceHelper IsSequenceRepeating(this IAsyncEnumerable<int> sequence, IAsyncEnumerable<int> pattern)
        => new(sequence, pattern);

    public class RepeatingSequenceHelper
    {
        private readonly IAsyncEnumerable<int> _sequence;
        private readonly IAsyncEnumerable<int> _pattern;

        internal RepeatingSequenceHelper(IAsyncEnumerable<int> sequence, IAsyncEnumerable<int> pattern)
            => (_sequence, _pattern) = (sequence, pattern);

        public async Task<bool> NTimes(int count)
            => await AsyncEnumerable
                .Range(0, count)
                .AggregateAwaitAsync(true, AggregateEquality);

        public async ValueTask<bool> AggregateEquality(bool b, int i)
            => b && await _sequence
                .Skip(i * await _pattern.CountAsync())
                .Zip(_pattern, (l, r) => l == r)
                .AllAsync(Identity);
    }
}
