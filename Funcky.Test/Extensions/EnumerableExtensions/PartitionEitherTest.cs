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
}
