namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class PartitionEitherTest
{
    [Fact]
    public void ReturnsTwoEmptyEnumerablesWhenSourceIsEmpty()
    {
        var (left, right) = Enumerable.Empty<Either<int, string>>().Partition();
        Assert.Empty(left);
        Assert.Empty(right);
    }

    [Fact]
    public void PartitionsItemsIntoLeftAndRight()
    {
        var input = Sequence.Return(
            Either<int, string>.Left(10),
            Either<int, string>.Right("a"),
            Either<int, string>.Right("b"),
            Either<int, string>.Left(20));
        var (left, right) = input.Partition();
        Assert.Equal(new[] { 10, 20 }, left);
        Assert.Equal(new[] { "a", "b" }, right);
    }

    [Fact]
    public void PartitionsItemsIntoLeftAndRightWithSelector()
    {
        var input = Enumerable.Range(0, count: 10).Materialize();
        var (left, right) = input.Partition(n => IsEven(n) ? Either<int, int>.Right(n) : Either<int, int>.Left(n));
        Assert.Equal(input.Where(Not<int>(IsEven)), left);
        Assert.Equal(input.Where(IsEven), right);
    }

    private static bool IsEven(int n) => n % 2 == 0;
}
