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
}
