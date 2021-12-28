using FsCheck;
using static Funcky.Functional;

namespace Funcky.Async.Test.TestUtilities
{
    public static class RepeatingSequence
    {
        public static RepeatingSequenceHelper IsSequenceRepeating(this IAsyncEnumerable<int> sequence, IEnumerable<int> pattern)
            => new(sequence, pattern);

        public class RepeatingSequenceHelper
        {
            private readonly IAsyncEnumerable<int> _sequence;
            private readonly IEnumerable<int> _pattern;

            internal RepeatingSequenceHelper(IAsyncEnumerable<int> sequence, IEnumerable<int> pattern)
                => (_sequence, _pattern) = (sequence, pattern);

            public Property NTimes(int count)
                => NTimesAsync(count).Result.ToProperty();

            public async ValueTask<bool> NTimesAsync(int count)
                => await Enumerable
                    .Range(0, count)
                    .ToAsyncEnumerable()
                    .AggregateAwaitAsync(true, IsOneRepetitionAsync);

            private async ValueTask<bool> IsOneRepetitionAsync(bool isRepeating, int count)
                => isRepeating && await _sequence
                    .Skip(count * _pattern.Count())
                    .Zip(_pattern.ToAsyncEnumerable(), (l, r) => l == r)
                    .AllAsync(Identity);
        }
    }
}
