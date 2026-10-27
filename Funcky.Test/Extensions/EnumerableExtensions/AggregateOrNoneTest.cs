using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class AggregateOrNoneTest
{
    [Property]
    public Property AggregateOrNoneGivesTheSameResultAsAggregateForNonEmptySequences(NonEmptyArray<int> sequence, Func<int, int, int> func)
        => (Option.Some(sequence.Get.Aggregate(func)) == sequence.Get.AggregateOrNone(func)).ToProperty();

    [Fact]
    public void AggregateOrNoneReturnsNoneForAnEmptySequence()
        => Assert.None(Enumerable.Empty<int>().AggregateOrNone((a, b) => a + b));

    [Fact]
    public void AggregateOrNoneReturnsTheOnlyElementWithoutInvokingTheAccumulator()
    {
        var accumulatorInvoked = false;

        var result = Enumerable.Return(42).AggregateOrNone((a, b) =>
        {
            accumulatorInvoked = true;
            return a + b;
        });

        Assert.Some(42, result);
        Assert.False(accumulatorInvoked);
    }

    [Fact]
    public void AggregateOrNoneFoldsFromTheLeft()
        => Assert.Some("((a,b),c)", Enumerable.Return("a", "b", "c").AggregateOrNone((a, b) => $"({a},{b})"));
}
