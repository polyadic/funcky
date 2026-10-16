using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.FuncExtensions;

public sealed class PredicateCompositionTest
{
    private static readonly Func<int, bool> IsEven = n => n % 2 == 0;
    private static readonly Func<int, bool> IsPositive = n => n > 0;
    private static readonly Func<int, bool> IsSmall = n => n < 10;

    [Property]
    public Property AndIsTheSameAsAll(int value)
        => (IsEven.And(IsPositive, IsSmall)(value) == All(IsEven, IsPositive, IsSmall)(value)).ToProperty();

    [Property]
    public Property OrIsTheSameAsAny(int value)
        => (IsEven.Or(IsPositive, IsSmall)(value) == Any(IsEven, IsPositive, IsSmall)(value)).ToProperty();

    [Property]
    public Property NotIsTheSameAsNot(int value)
        => (IsEven.Not()(value) == Not(IsEven)(value)).ToProperty();

    [Property]
    public Property AndAndOrWithoutFurtherPredicatesAreTheReceiver(int value)
        => (IsEven.And()(value) == IsEven(value) && IsEven.Or()(value) == IsEven(value)).ToProperty();

    [Fact]
    public void TheTypeArgumentIsInferredFromTheReceiver()
    {
        // Compiles without a type argument, which Not<string>(False) needs.
        var notEmpty = Fn<Func<string, bool>>(string.IsNullOrEmpty).Not();

        Assert.True(notEmpty("foo"));
        Assert.False(notEmpty(string.Empty));
    }

    [Fact]
    public void AndShortCircuitsLikeTheConditionalAndOperator()
    {
        var evaluated = false;
        Func<int, bool> spy = _ => evaluated = true;

        Assert.False(IsEven.And(spy)(1));
        Assert.False(evaluated);
    }

    [Fact]
    public void OrShortCircuitsLikeTheConditionalOrOperator()
    {
        var evaluated = false;
        Func<int, bool> spy = _ => evaluated = true;

        Assert.True(IsEven.Or(spy)(2));
        Assert.False(evaluated);
    }

    [Fact]
    public void PredicatesComposeFurther()
    {
        var evenOrSmallButNotPositive = IsEven.Or(IsSmall).And(IsPositive.Not());

        Assert.True(evenOrSmallButNotPositive(-2));
        Assert.True(evenOrSmallButNotPositive(-3));
        Assert.False(evenOrSmallButNotPositive(4));
    }
}
