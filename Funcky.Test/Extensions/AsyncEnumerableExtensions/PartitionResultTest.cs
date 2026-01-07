namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class PartitionResultTest
{
    [Fact]
    public async Task ReturnsTwoEmptyListsWhenSourceIsEmpty()
    {
        var (error, ok) = await AsyncEnumerable.Empty<Either<int, string>>().PartitionAsync();
        Assert.Empty(error);
        Assert.Empty(ok);
    }

    [Fact]
    public async Task PartitionsItemsIntoOkAndError()
    {
        var values = Sequence.Return(10, 20);
        var exceptions = Sequence.Return(new Exception("foo"), new InvalidOperationException("bar"));
        var input = values.Select(Result.Ok).ToAsyncEnumerable().Interleave(exceptions.Select(Result<int>.Error).ToAsyncEnumerable());

        var (error, ok) = await input.PartitionAsync();

        Assert.Equal(values, ok);
        Assert.Equal(exceptions, error);
    }
}
