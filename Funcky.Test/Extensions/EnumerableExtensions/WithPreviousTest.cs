using System.Collections.Immutable;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Test.TestUtils;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WithPreviousTest
    {
        [Fact]
        public void WithPreviousIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.WithPrevious();
        }

        [Fact]
        public void AnEmptySequenceWithPreviousReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();
            Assert.Empty(emptySequence.WithPrevious());
        }

        [Fact]
        public void ASequenceWithOneElementWithPreviousHasOneElementWithNoPreviousElement()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = Sequence.Return(expectedValue);
            var sequenceWithPrevious = oneElementSequence.WithPrevious();
            Assert.Collection(sequenceWithPrevious, value =>
            {
                Assert.Equal(expectedValue, value.Value);
                FunctionalAssert.IsNone(value.Previous);
            });
        }

        [Fact]
        public void ASequenceWithMoreThanOneElementWithPreviousHasPreviousSetExceptOnFirstElement()
        {
            var sequence = ImmutableArray.Create("foo", "bar", "baz", "qux");
            var expectedSequenceWithPrevious = ImmutableArray.Create(
                new ValueWithPrevious<string>("foo", Option<string>.None()),
                new ValueWithPrevious<string>("bar", "foo"),
                new ValueWithPrevious<string>("baz", "bar"),
                new ValueWithPrevious<string>("qux", "baz"));
            Assert.Equal(expectedSequenceWithPrevious, sequence.WithPrevious());
        }
    }
}
