namespace Funcky.Test;

public sealed class SuccessorsTest
{
    [Fact]
    public void ReturnsEmptySequenceWhenFirstItemIsNone()
    {
        Assert.Empty(Enumerable.Successors(Option<int>.None, Identity));
    }

    [Fact]
    public void ReturnsOnlyTheFirstItemWhenSuccessorFunctionImmediatelyReturnsNone()
    {
        var first = Assert.Single(Enumerable.Successors(10, _ => Option<int>.None));
        Assert.Equal(10, first);
    }

    [Fact]
    public void SuccessorsWithNonOptionFunctionReturnsEndlessEnumerable()
    {
        const int count = 40;
        Assert.Equal(count, Enumerable.Successors(0, Identity).Take(count).Count());
    }

    [Fact]
    public void SuccessorsReturnsEnumerableThatReturnsValuesBasedOnSeed()
    {
        Assert.Equal(
            Enumerable.Range(0, 10),
            Enumerable.Successors(0, i => i + 1).Take(10));
    }

    [Fact]
    public void SuccessorsReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFunc()
    {
        Assert.Equal(
            Enumerable.Range(0, 11),
            Enumerable.Successors(0, i => (i < 10).Then(i + 1)));
    }

    [Fact]
    public void CanGenerateFibonacciSequence()
    {
        var fibonacci = Enumerable.Successors((0, 1), n => (n.Item2, n.Item1 + n.Item2)).Select(n => n.Item1);
        Assert.Equal(Enumerable.Return(0, 1, 1, 2, 3, 5, 8, 13, 21, 34), fibonacci.Take(10));
    }

    [Fact]
    public void ANullSuccessorFunctionIsRejectedWhenTheMethodIsCalled()
    {
        Assert.Equal("successor", Assert.Throws<ArgumentNullException>(() => Enumerable.Successors(1, (Func<int, int>)null!)).ParamName);
        Assert.Equal("successor", Assert.Throws<ArgumentNullException>(() => Enumerable.Successors(1, (Func<int, Option<int>>)null!)).ParamName);
        Assert.Equal("successor", Assert.Throws<ArgumentNullException>(() => Enumerable.Successors(Option.Some(1), (Func<int, int>)null!)).ParamName);
        Assert.Equal("successor", Assert.Throws<ArgumentNullException>(() => Enumerable.Successors(Option.Some(1), (Func<int, Option<int>>)null!)).ParamName);
    }
}
