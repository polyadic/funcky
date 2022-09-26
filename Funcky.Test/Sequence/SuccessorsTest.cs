namespace Funcky.Test;

public sealed class SuccessorsTest
{
    [Fact]
    public void ReturnsEmptySequenceWhenFirstItemIsNone()
    {
        Assert.Empty(Sequence.Successors(Option<int>.None, Identity));
    }

    [Fact]
    public void ReturnsOnlyTheFirstItemWhenSuccessorFunctionImmediatelyReturnsNone()
    {
        var first = Assert.Single(Sequence.Successors(10, _ => Option<int>.None));
        Assert.Equal(10, first);
    }

    [Fact]
    public void SuccessorsWithNonOptionFunctionReturnsEndlessEnumerable()
    {
        const int count = 40;
        Assert.Equal(count, Sequence.Successors(0, Identity).Take(count).Count());
    }

    [Fact]
    public void SuccessorsReturnsEnumerableThatReturnsValuesBasedOnSeed()
    {
        Assert.Equal(
            Enumerable.Range(0, 10),
            Sequence.Successors(0, i => i + 1).Take(10));
    }

    [Fact]
    public void SuccessorsReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFunc()
    {
        Assert.Equal(
            Enumerable.Range(0, 11),
            Sequence.Successors(0, i => Option.FromBoolean(i < 10, i + 1)));
    }

    [Fact]
    public void CanGenerateFibonacciSequence()
    {
        var fibonacci = Sequence.Successors((0, 1), n => (n.Item2, n.Item1 + n.Item2)).Select(n => n.Item1);
        Assert.Equal(Sequence.Return(0, 1, 1, 2, 3, 5, 8, 13, 21, 34), fibonacci.Take(10));
    }
}
