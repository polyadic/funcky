using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class TransposeTest
{
    [Fact]
    public async Task TransposeIsLazyElementsGetOnlyEnumeratedWhenRequested()
    {
        const int numberOfRows = 5;
        const int numberOfColumns = 3;
        var lazyMatrix = LazyMatrix(numberOfRows, numberOfColumns);

        var transposedMatrix = lazyMatrix.Transpose();

        Assert.Equal(0, CountCreation.Count);

        await transposedMatrix.ForEachAsync(row => _ = row.ToList());

        Assert.Equal(numberOfRows * numberOfColumns, CountCreation.Count);
    }

    [Fact]
    public async Task TransposingAnEmptyMatrixResultsInAnEmptyMatrix()
    {
        var emptyMatrix = Enumerable.Empty<IAsyncEnumerable<int>>();

        var transposedMatrix = emptyMatrix.Transpose();

        await AsyncAssert.Empty(transposedMatrix);
    }

    [Fact]
    public async Task TransposingAMatrixResultsInATransposedMatrixAsync()
    {
        var transposed = MatrixExample().Transpose();

        await AsyncAssert.Collection(
            transposed,
            row => { Assert.Equal([1, 5, 9], row); },
            row => { Assert.Equal([2, 6, 10], row); },
            row => { Assert.Equal([3, 7, 11], row); },
            row => { Assert.Equal([4, 8, 12], row); });
    }

    [Fact]
    public void GivenAMagicSquareTransposeDoesNotChangeTheAverages()
    {
        MagicSquare()
            .Select(x => x.AverageAsync())
            .ForEach(async average => Assert.Equal(5, await average));

        MagicSquare()
            .Transpose()
            .Select(Enumerable.Average)
            .ForEachAsync(average => Assert.Equal(5, average));
    }

    [Fact]
    public async Task GivenAJaggedArrayTheTransposeDoesNotWorkAsExpected()
    {
        // Jagged sequences do not work!
        // If you use jagged sequences, in Transpose you are using an implementation detail which could change.
        var transposed = JaggedMatrixExample().Transpose();

        await AsyncAssert.Collection(
            transposed,
            row => { Assert.Equal([1, 6, 5, 10], row); },
            row => { Assert.Equal([2, 9, 3, 42], row); },
            row => { Assert.Equal([4], row); });
    }

    private static IEnumerable<IAsyncEnumerable<int>> MagicSquare()
        =>
        [
            AsyncSequence.Return(4, 9, 2),
            AsyncSequence.Return(3, 5, 7),
            AsyncSequence.Return(8, 1, 6),
        ];

    private static IEnumerable<IAsyncEnumerable<int>> MatrixExample()
        =>
        [
            AsyncSequence.Return(1, 2, 3, 4),
            AsyncSequence.Return(5, 6, 7, 8),
            AsyncSequence.Return(9, 10, 11, 12)
        ];

    private static IEnumerable<IAsyncEnumerable<int>> JaggedMatrixExample()
        =>
        [
            AsyncSequence.Return(1, 2, 3, 4),
            AsyncSequence.Return(6, 9, 42),
            AsyncSequence.Return(5),
            AsyncSequence.Return(10)
        ];

    private static IEnumerable<IAsyncEnumerable<CountCreation>> LazyMatrix(int rows, int columns) =>
        from row in Enumerable.Range(0, rows)
        select from column in AsyncEnumerable.Range(0, columns)
               select new CountCreation();
}
