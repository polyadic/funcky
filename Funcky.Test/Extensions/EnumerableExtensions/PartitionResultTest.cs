namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class PartitionResultTest
{
    [Fact]
    public void ReturnsTwoEmptyEnumerablesWhenSourceIsEmpty()
    {
        var (left, right) = Enumerable.Empty<Either<int, string>>().Partition();
        Assert.Empty(left);
        Assert.Empty(right);
    }

    [Fact]
    public void PartitionsItemsIntoOkAndError()
    {
        var values = Sequence.Return(10, 20);
        var exceptions = Sequence.Return(new Exception("foo"), new InvalidOperationException("bar"));
        var input = values.Select(Result.Ok).Interleave(exceptions.Select(Result<int>.Error));

        var (ok, error) = input.Partition();

        Assert.Equal(values, ok);
        Assert.Equal(exceptions, error);
    }
}
