using Funcky.Async.Extensions;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class PartitionEitherTest
{
    [Fact]
    public async Task ReturnsTwoEmptyListsWhenSourceIsEmpty()
    {
        var (left, right) = await AsyncEnumerable.Empty<Either<int, string>>().PartitionAsync();
        Assert.Empty(left);
        Assert.Empty(right);
    }

    [Fact]
    public async Task PartitionsItemsIntoLeftAndRight()
    {
        var input = AsyncSequence.Return(
            Either<int, string>.Left(10),
            Either<int, string>.Right("a"),
            Either<int, string>.Right("b"),
            Either<int, string>.Left(20));
        var (left, right) = await input.PartitionAsync();
        Assert.Equal(new[] { 10, 20 }, left);
        Assert.Equal(new[] { "a", "b" }, right);
    }

    [Fact]
    public async Task PartitionsItemsIntoLeftAndRightWithSelector()
    {
        var input = Enumerable.Range(0, count: 10).Materialize();
        var (left, right) = await input.ToAsyncEnumerable().PartitionAsync(n => IsEven(n) ? Either<int, int>.Right(n) : Either<int, int>.Left(n));
        Assert.Equal(input.Where(Not<int>(IsEven)), left);
        Assert.Equal(input.Where(IsEven), right);
    }

    private static bool IsEven(int n) => n % 2 == 0;
}
