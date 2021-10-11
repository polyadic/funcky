using System.Collections.Immutable;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class WithPreviousTest
    {
        [Fact]
        public void WithPreviousIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            _ = doNotEnumerate.WithPrevious();
        }

        [Fact]
        public async Task AnEmptySequenceWithPreviousReturnsAnEmptySequence()
        {
            var emptySequence = AsyncEnumerable.Empty<string>();
            await AsyncAssert.Empty(emptySequence.WithPrevious());
        }

        [Fact]
        public async Task ASequenceWithOneElementWithPreviousHasOneElementWithNoPreviousElement()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = AsyncSequence.Return(expectedValue);
            var sequenceWithPrevious = oneElementSequence.WithPrevious();
            await AsyncAssert.Collection(sequenceWithPrevious, value =>
            {
                Assert.Equal(expectedValue, value.Value);
                FunctionalAssert.IsNone(value.Previous);
            });
        }

        [Fact]
        public async Task ASequenceWithMoreThanOneElementWithPreviousHasPreviousSetExceptOnFirstElement()
        {
            var sequence = ImmutableArray.Create("foo", "bar", "baz", "qux").ToAsyncEnumerable();
            var expectedSequenceWithPrevious = ImmutableArray.Create(
                new ValueWithPrevious<string>("foo", Option<string>.None),
                new ValueWithPrevious<string>("bar", "foo"),
                new ValueWithPrevious<string>("baz", "bar"),
                new ValueWithPrevious<string>("qux", "baz")).ToAsyncEnumerable();

            Assert.Equal(await expectedSequenceWithPrevious.ToListAsync(), await sequence.WithPrevious().ToListAsync());
        }
    }
}
