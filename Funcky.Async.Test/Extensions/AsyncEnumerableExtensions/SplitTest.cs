using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class SplitTest
    {
        [Fact]
        public void SplitIsAnIAsyncEnumerableLazily()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            _ = doNotEnumerate.Split(42);
        }

        [Property]
        public Property SplittingAnEmptyIAsyncEnumerableAlwaysReturnsAnEmptyEnumerable(int separator)
        {
            var parts = AsyncEnumerable.Empty<int>().Split(separator);

            return (!parts.AnyAsync().Result).ToProperty();
        }

        [Fact]
        public async Task SplitAnIAsyncEnumerableCorrectly()
        {
            var sequence = AsyncSequence.Return(12, 14, 7, 41, 31, 19, 7, 9, 11, 99, 99);

            var parts = sequence.Split(7);

            var expected = AsyncSequence.Return(
                Sequence.Return(12, 14),
                Sequence.Return(41, 31, 19),
                Sequence.Return(9, 11, 99, 99));

            await AsyncAssert.Equal(expected, parts);
        }
    }
}
