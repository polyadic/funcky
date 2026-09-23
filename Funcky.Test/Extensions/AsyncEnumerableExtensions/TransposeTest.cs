#if INTEGRATED_ASYNC
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class TransposeTest
{
    [Fact]
    public async Task TransposeEnumeratesTheOuterSequenceLazilyAndOnlyOnce()
    {
        var enumerations = 0;

        var transposed = CountingMatrix(() => enumerations++).Transpose();
        Assert.Equal(0, enumerations);

        await transposed.ToListAsync();
        Assert.Equal(1, enumerations);
    }

    [Fact]
    public async Task TransposeEnumeratesTheInnerSequencesLazily()
    {
        var enumeratedElements = 0;
        var lazyMatrix = Enumerable.Range(0, 5).Select(_ => AsyncEnumerable.Range(0, 3).Select(_ => enumeratedElements++));

        await using var columns = lazyMatrix.Transpose().GetAsyncEnumerator();
        Assert.Equal(0, enumeratedElements);

        Assert.True(await columns.MoveNextAsync());
        Assert.Equal(5, enumeratedElements);

        Assert.True(await columns.MoveNextAsync());
        Assert.Equal(10, enumeratedElements);
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
    public async Task GivenAMagicSquareTransposeDoesNotChangeTheAverages()
    {
        MagicSquare()
            .Select(x => x.AverageAsync())
            .ForEach(async average => Assert.Equal(5, await average));

        var averages = MagicSquare()
            .Transpose()
            .Select(Enumerable.Average);

        await foreach (var average in averages)
        {
            Assert.Equal(5, average);
        }
    }

    [Fact]
    public async Task TransposingAJaggedMatrixThrows()
    {
        var transposed = JaggedMatrixExample().Transpose();

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await transposed.ToListAsync());
    }

    [Fact]
    public async Task TransposingAJaggedMatrixYieldsTheRectangularColumnsBeforeThrowing()
    {
        var jaggedMatrix = Sequence.Return(AsyncSequence.Return(1, 2, 3), AsyncSequence.Return(4, 5));

        await using var columns = jaggedMatrix.Transpose().GetAsyncEnumerator();

        Assert.True(await columns.MoveNextAsync());
        Assert.Equal([1, 4], columns.Current);
        Assert.True(await columns.MoveNextAsync());
        Assert.Equal([2, 5], columns.Current);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await columns.MoveNextAsync());
    }

    [Fact]
    public async Task TransposingAMatrixWithEmptyRowsResultsInAnEmptyMatrix()
    {
        var matrix = Sequence.Return(AsyncEnumerable.Empty<int>(), AsyncEnumerable.Empty<int>());

        await AsyncAssert.Empty(matrix.Transpose());
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

    private static IEnumerable<IAsyncEnumerable<int>> CountingMatrix(Action onEnumeration)
    {
        onEnumeration();

        yield return AsyncSequence.Return(1, 2, 3, 4);
        yield return AsyncSequence.Return(5, 6, 7, 8);
        yield return AsyncSequence.Return(9, 10, 11, 12);
    }
}
#endif
