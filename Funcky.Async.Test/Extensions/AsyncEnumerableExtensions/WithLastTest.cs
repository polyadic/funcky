using System.Linq;
using System.Threading.Tasks;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WithLastTest
    {
        [Fact]
        public void WithLastIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            _ = doNotEnumerate.WithLast();
        }

        [Fact]
        public async Task AnEmptySequenceWithLastReturnsAnEmptySequence()
        {
            var emptySequence = AsyncEnumerable.Empty<string>();

            Assert.False(await emptySequence.WithLast().AnyAsync());
        }

        [Fact]
        public async Task ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedLast()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = AsyncSequence.Return(expectedValue);

            var sequenceWithLast = oneElementSequence.WithLast();
            await foreach (var (value, isLast) in sequenceWithLast)
            {
                Assert.Equal(expectedValue, value);
                Assert.True(isLast);
            }

            Assert.True(await sequenceWithLast.AnyAsync());
        }

        [Fact]
        public async Task ASequenceWithMultipleElementsWithLastMarksTheLastElement()
        {
            const int length = 20;
            var sequence = AsyncEnumerable.Range(1, length);

            await foreach (var valueWithLast in sequence.WithLast())
            {
                Assert.Equal(valueWithLast.Value == length, valueWithLast.IsLast);
            }
        }
    }
}
