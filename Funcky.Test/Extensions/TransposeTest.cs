using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public class TransposeTest
    {
        [Fact]
        public void TransposingAnEmptyMatrixResultsInAnEmptyMatrix()
        {
            var emptyMatrix = Enumerable.Empty<IEnumerable<int>>();

            var transposedMatrix = emptyMatrix.Transpose();

            Assert.Empty(transposedMatrix);
        }

        [Fact]
        public void GivenAnSingleElementListWeGetEnumerableWithOneElement()
        {
            var transposed = GetMatrixExample().Transpose();

            Assert.Collection(
                transposed,
                row1 =>
                {
                    Assert.Collection(
                        row1,
                        column1 => Assert.Equal(1, column1),
                        column2 => Assert.Equal(5, column2),
                        column3 => Assert.Equal(9, column3));
                },
                row2 =>
                {
                    Assert.Collection(
                        row2,
                        column1 => Assert.Equal(2, column1),
                        column2 => Assert.Equal(6, column2),
                        column3 => Assert.Equal(10, column3));
                },
                row3 =>
                {
                    Assert.Collection(
                        row3,
                        column1 => Assert.Equal(3, column1),
                        column2 => Assert.Equal(7, column2),
                        column3 => Assert.Equal(11, column3));
                },
                row4 =>
                {
                    Assert.Collection(
                        row4,
                        column1 => Assert.Equal(4, column1),
                        column2 => Assert.Equal(8, column2),
                        column3 => Assert.Equal(12, column3));
                });
        }

        private static IEnumerable<IEnumerable<int>> GetMatrixExample()
        {
            return new List<IEnumerable<int>>()
            {
                new List<int>() { 1, 2, 3, 4 },
                new List<int>() { 5, 6, 7, 8 },
                new List<int>() { 9, 10, 11, 12 },
            };
        }
    }
}
