using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class MaterializeTest
    {
        [Fact]
        public async Task MaterializeEnumeratesNonCollection()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            await Assert.ThrowsAsync<XunitException>(async () => await doNotEnumerate.Materialize());
        }

        [Fact]
        public async Task MaterializeASequenceReturnsAListByDefault()
        {
            var sequence = AsyncEnumerable.Repeat("Hello world!", 3);

            Assert.IsType<List<string>>(await sequence.Materialize());
        }

        [Fact]
        public async Task MaterializeWithMaterializationReturnsCorrectCollectionWhenEnumerate()
        {
            var sequence = AsyncEnumerable.Repeat("Hello world!", 3);

            Assert.IsType<HashSet<string>>(await sequence.Materialize(ToHashSet));
        }

        private static ValueTask<HashSet<string>> ToHashSet(IAsyncEnumerable<string> s)
            => s.ToHashSetAsync();
    }
}
