namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class PartitionResultTest
{
    [Fact]
    public void ReturnsTwoEmptyEnumerablesWhenSourceIsEmpty()
    {
        var (error, ok) = Enumerable.Empty<Either<int, string>>().Partition();
        Assert.Empty(error);
        Assert.Empty(ok);
    }

    [Fact]
    public void PartitionsItemsIntoOkAndError()
    {
        var values = Sequence.Return(10, 20);
        var exceptions = Sequence.Return(new Exception("foo"), new InvalidOperationException("bar"));
        var input = values.Select(Result.Ok).Interleave(exceptions.Select(Result<int>.Error));

        var (error, ok) = input.Partition();

        Assert.Equal(values, ok);
        Assert.Equal(exceptions, error);
    }
}
