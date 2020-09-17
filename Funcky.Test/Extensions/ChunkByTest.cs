using System;
using System.Collections.Generic;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test
{
    public class ChunkByTest
    {
        [Fact]
        public void GivenAnEmptyEnumerableChunkByWithTruePredicateReturnsAnEmptyList()
        {
            List<int> numbers = new List<int>();

            var chunked = numbers.ChunkBy(Functional.True);

            Assert.Empty(chunked);
        }

        [Fact]
        public void GivenAnEmptyEnumerableChunkByWithFalsePredicateReturnsAnEmptyList()
        {
            List<int> numbers = new List<int>();

            var chunked = numbers.ChunkBy(Functional.False);

            Assert.Empty(chunked);
        }

        [Fact]
        public void GivenAnSingleElementListChunkByWithTruePredicateReturnsEnumerbaleWithOneElement()
        {
            List<int> numbers = new List<int> { 1 };

            var chunked = numbers.ChunkBy(Functional.True);

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
        public void GivenAnSingleElementListChunkByWithFalsePredicateReturnsEnumerbaleWithOneElement()
        {
            List<int> numbers = new List<int> { 1 };

            var chunked = numbers.ChunkBy(Functional.False);

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
        public void GivenAAlwaysFalsePredicateChunkByReturnsChunksWithSingleElements()
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4 };

            var chunked = numbers.ChunkBy(Functional.True);

            Assert.Collection(
                chunked,
                a =>
                {
                    Assert.Collection(
                        a,
                        aa => Assert.Equal(1, aa));
                },
                b =>
                {
                    Assert.Collection(
                        b,
                        bb => Assert.Equal(2, bb));
                },
                c =>
                {
                    Assert.Collection(
                        c,
                        cc => Assert.Equal(3, cc));
                },
                d =>
                {
                    Assert.Collection(
                        d,
                        dd => Assert.Equal(4, dd));
                });
        }
    }
}
