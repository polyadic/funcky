using System;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Test.TestUtils;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class ZipLongestTest
    {
        [Fact]
        public void ZipLongestIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.ZipLongest(doNotEnumerate, (_, _) => Unit.Value);
        }

        [Fact]
        public void GivenTwoEmptySequencesZipLongestReturnsAnEmptySequence()
        {
            var numbers = Enumerable.Empty<int>();
            var strings = Enumerable.Empty<string>();

            var zipped = numbers.ZipLongest(strings, (_, _) => Unit.Value);

            Assert.Empty(zipped);
        }

        [Fact]
        public void GivenTwoSequencesOfTheSameLengthWeGetNoNoneValue()
        {
            var numbers = Enumerable.Range(0, 3);
            var strings = new[] { "Alpha", "Beta", "Gamma" };

            var zipped = numbers
                .ZipLongest(strings)
                .ToList();

            Assert.Equal(3, zipped.Count);
            foreach (var value in zipped)
            {
                _ = FunctionalAssert.IsSome(value.First);
                _ = FunctionalAssert.IsSome(value.Second);
            }
        }

        [Fact]
        public void GivenTwoSequencesWeOfDifferentLengthWeGetTheLongerAndFillWithNone()
        {
            var numbers = Enumerable.Range(0, 10);
            var strings = new[] { "Alpha", "Beta", "Gamma" };

            var zipped = numbers
                .ZipLongest(strings, ValueTuple.Create)
                .ToList();

            Assert.Equal(10, zipped.Count);
            Assert.Equal((Option.Some(0), Option.Some("Alpha")), zipped.First());
            Assert.Equal((Option.Some(9), Option<string>.None()), zipped.Last());
        }
    }
}
