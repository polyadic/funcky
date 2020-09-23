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
        public void TransposingAMatrixResultsInATransposedMatrix()
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

        [Fact]
        public void TransposeIsLazyElementsGetOnlyEnumeratedWhenRequested()
        {
            var numberOfRows = 5;
            var numberOfColumns = 3;
            var lazyMatric = LazyMatrix(numberOfRows, numberOfColumns);

            var transposedMatrix = lazyMatric.Transpose();

            Assert.Equal(0, CountCreation.Count);

            foreach (var row in transposedMatrix)
            {
                foreach (var column in row)
                {
                }
            }

            Assert.Equal(numberOfColumns * numberOfColumns, CountCreation.Count);
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

        private IEnumerable<IEnumerable<CountCreation>> LazyMatrix(int rows, int columns)
        {
            foreach (var row in Enumerable.Range(0, rows))
            {
                yield return LazyRow(columns);
            }
        }

        private IEnumerable<CountCreation> LazyRow(int columns)
        {
            foreach (var column in Enumerable.Range(0, columns))
            {
                yield return new CountCreation();
            }
        }
    }
}
