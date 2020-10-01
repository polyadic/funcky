using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
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
            var transposed = MatrixExample().Transpose();

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
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "The ToList just enumerates the items, we test for the side effect in the next line.")]
        public void TransposeIsLazyElementsGetOnlyEnumeratedWhenRequested()
        {
            var numberOfRows = 5;
            var numberOfColumns = 3;
            var lazyMatrix = LazyMatrix(numberOfRows, numberOfColumns);

            var transposedMatrix = lazyMatrix.Transpose();

            Assert.Equal(0, CountCreation.Count);

            transposedMatrix.ForEach(row => row.ToList());

            Assert.Equal(numberOfRows * numberOfColumns, CountCreation.Count);
        }

        [Fact]
        public void GivenAMagicSquareTransposeDoesNotChangeTheAverages()
        {
            MagicSquare()
                .Select(Enumerable.Average)
                .ForEach(average => Assert.Equal(5, average));

            MagicSquare()
                .Transpose()
                .Select(Enumerable.Average)
                .ForEach(average => Assert.Equal(5, average));
        }

        [Fact]
        public void GivenASpecialJaggedArrayTheTransposeWorks()
        {
            // Jagged sequences generally do not work!
            // If you use jagged sequences, in Transpose you are using an implementation detail.
            var transposed = JaggedMatrixExample().Transpose();

            Assert.Collection(
                transposed,
                row1 => { Assert.Equal(new[] { 1, 6, 5, 10 }, row1); },
                row2 => { Assert.Equal(new[] { 2, 9 }, row2); },
                row3 => { Assert.Equal(new[] { 3 }, row3); },
                row4 => { Assert.Equal(new[] { 4 }, row4); });
        }

        private static IEnumerable<IEnumerable<int>> MagicSquare() =>
            new List<IEnumerable<int>>
            {
                new List<int> { 4, 9, 2 },
                new List<int> { 3, 5, 7 },
                new List<int> { 8, 1, 6 },
            };

        private static IEnumerable<IEnumerable<int>> MatrixExample() =>
            new List<IEnumerable<int>>
            {
                new List<int> { 1, 2, 3, 4 },
                new List<int> { 5, 6, 7, 8 },
                new List<int> { 9, 10, 11, 12 },
            };

        private static IEnumerable<IEnumerable<int>> JaggedMatrixExample() =>
            new List<IEnumerable<int>>
            {
                new List<int> { 1, 2, 3, 4 },
                new List<int> { 6, 9 },
                new List<int> { 5 },
                new List<int> { 10 },
            };

        private IEnumerable<IEnumerable<CountCreation>> LazyMatrix(int rows, int columns) =>
            from row in Enumerable.Range(0, rows)
            select from column in Enumerable.Range(0, columns)
                select new CountCreation();
    }
}
