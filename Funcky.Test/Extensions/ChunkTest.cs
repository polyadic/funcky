using System.Collections.Generic;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public class ChunkTest
    {
        [Fact]
        public void GivenAnEmptyEnumerableChunkReturnsAnEmptyList()
        {
            List<int> numbers = new List<int>();

            var chunked = numbers.Chunk(3);

            Assert.Empty(chunked);
        }

        [Fact]
        public void GivenAnSingleElementListWeGetEnumerbaleWithOneElement()
        {
            List<int> numbers = new List<int> { 1 };

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
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

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
    }
}
