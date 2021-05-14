using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class ChunkTest
    {
        [Fact]
        public void ChunkIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.Chunk(42);
        }

        [Fact]
        public void GivenAnEmptyEnumerableChunkReturnsAnEmptyList()
        {
            var numbers = Enumerable.Empty<int>();

            var chunked = numbers.Chunk(3);

            Assert.Empty(chunked);
        }

        [Fact]
        public void GivenAnSingleElementListWeGetEnumerableWithOneElement()
        {
            var numbers = new List<int> { 1 };

            var chunked = numbers.Chunk(3);

            Assert.Collection(
                chunked,
                a =>
                {
                    Assert.Collection(
                        a,
                        aa => Assert.Equal(1, aa));
                });
        }

        [Fact]
        public void GivenAnEnumerableWeChanChunkItIntoAnEnumerableOfEnumerables()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var chunked = numbers.Chunk(3);

            Assert.Collection(
                chunked,
                a =>
                {
                    Assert.Collection(
                        a,
                        aa => Assert.Equal(1, aa),
                        ab => Assert.Equal(2, ab),
                        ac => Assert.Equal(3, ac));
                },
                b =>
                {
                    Assert.Collection(
                        b,
                        ba => Assert.Equal(4, ba),
                        bb => Assert.Equal(5, bb),
                        bc => Assert.Equal(6, bc));
                },
                c =>
                {
                    Assert.Collection(
                        c,
                        ca => Assert.Equal(7, ca),
                        cb => Assert.Equal(8, cb),
                        cc => Assert.Equal(9, cc));
                });
        }

        [Fact]
        public void GivenAnEnumerableNotAMultipleOfSizeWeHaveASmallerLastSlice()
        {
            var numbers = new List<string> { "a", "b", "c", "d", "e", "g", "h", "i", "j" };

            var chunkSize = 4;
            var chunked = numbers.Chunk(chunkSize);

            Assert.Collection(
                chunked,
                a =>
                {
                    Assert.Equal(a.Count(), chunkSize);
                },
                b =>
                {
                    Assert.Equal(b.Count(), chunkSize);
                },
                c =>
                {
                    Assert.Equal(c.Count(), numbers.Count % chunkSize);
                });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-42)]
        public void ChunkThrowsOnZeroOrNegativeChunkSizes(int invalidChunkSize)
        {
            var numbers = new List<int> { 1 };

            Assert.Throws<ArgumentOutOfRangeException>(() => Funcky.Extensions.EnumerableExtensions.Chunk(numbers, invalidChunkSize));
        }

        [Fact]
        public void ChunkWithResultSelectorAppliesTheSelectorCorrectlyToTheSubsequence()
        {
            var magicSquare = new List<int> { 4, 9, 2, 3, 5, 7, 8, 1, 6 };

            magicSquare
                .Chunk(3, Enumerable.Average)
                .ForEach(average => Assert.Equal(5, average));
        }
    }
}
