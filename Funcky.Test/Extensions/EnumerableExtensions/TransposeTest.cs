using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class TransposeTest
{
    [Fact]
    public void TransposeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<IEnumerable<int>>();

        _ = doNotEnumerate.Transpose();
    }

    [Fact]
    public void TransposeEnumeratesTheOuterSequenceOnlyOnce()
    {
        var transposed = EnumerateOnce.Create(MatrixExample()).Transpose();

        Assert.Equal([[1, 5, 9], [2, 6, 10], [3, 7, 11], [4, 8, 12]], transposed);
    }

    [Fact]
    public void TransposeEnumeratesTheInnerSequencesLazily()
    {
        var enumeratedElements = 0;
        var lazyMatrix = Enumerable.Range(0, 5).Select(_ => Enumerable.Range(0, 3).Select(_ => enumeratedElements++));

        using var columns = lazyMatrix.Transpose().GetEnumerator();
        Assert.Equal(0, enumeratedElements);

        Assert.True(columns.MoveNext());
        Assert.Equal(5, enumeratedElements);

        Assert.True(columns.MoveNext());
        Assert.Equal(10, enumeratedElements);
    }

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
            row => { Assert.Equal([1, 5, 9], row); },
            row => { Assert.Equal([2, 6, 10], row); },
            row => { Assert.Equal([3, 7, 11], row); },
            row => { Assert.Equal([4, 8, 12], row); });
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
    public void TransposingAJaggedMatrixThrows()
    {
        var transposed = JaggedMatrixExample().Transpose();

        Assert.Throws<InvalidOperationException>(() => transposed.ToList());
    }

    [Fact]
    public void TransposingAJaggedMatrixYieldsTheRectangularColumnsBeforeThrowing()
    {
        var jaggedMatrix = Sequence.Return(Sequence.Return(1, 2, 3), Sequence.Return(4, 5));

        using var columns = jaggedMatrix.Transpose().GetEnumerator();

        Assert.True(columns.MoveNext());
        Assert.Equal([1, 4], columns.Current);
        Assert.True(columns.MoveNext());
        Assert.Equal([2, 5], columns.Current);
        Assert.Throws<InvalidOperationException>(() => columns.MoveNext());
    }

    [Fact]
    public void TransposingAMatrixWithEmptyRowsResultsInAnEmptyMatrix()
    {
        var matrix = Sequence.Return(Enumerable.Empty<int>(), Enumerable.Empty<int>());

        Assert.Empty(matrix.Transpose());
    }

    private static IEnumerable<IEnumerable<int>> MagicSquare()
        => Sequence.Return(
            Sequence.Return(4, 9, 2),
            Sequence.Return(3, 5, 7),
            Sequence.Return(8, 1, 6));

    private static IEnumerable<IEnumerable<int>> MatrixExample()
        => Sequence.Return(
            Sequence.Return(1, 2, 3, 4),
            Sequence.Return(5, 6, 7, 8),
            Sequence.Return(9, 10, 11, 12));

    private static IEnumerable<IEnumerable<int>> JaggedMatrixExample()
        => Sequence.Return(
            Sequence.Return(1, 2, 3, 4),
            Sequence.Return(6, 9, 42),
            Sequence.Return(5),
            Sequence.Return(10));
}
