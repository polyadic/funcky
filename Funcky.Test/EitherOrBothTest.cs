using System.Diagnostics.CodeAnalysis;

namespace Funcky.Test;

public sealed class EitherOrBothTest
{
    [Fact]
    public void BothConstructorThrowsWhenNullIsPassed()
    {
        Assert.Throws<ArgumentNullException>(() => EitherOrBoth<string, string>.Both(null!, "right"));
        Assert.Throws<ArgumentNullException>(() => EitherOrBoth<string, string>.Both("left", null!));
    }

    [Fact]
    public void MatchSelectsTheConstructedSide()
    {
        Assert.Equal("left", EitherOrBoth<string, int>.Left("left").Match(left: Identity, right: static r => r.ToString(), both: static (l, r) => l + r));
        Assert.Equal("1", EitherOrBoth<string, int>.Right(1).Match(left: Identity, right: static r => r.ToString(), both: static (l, r) => l + r));
        Assert.Equal("left1", EitherOrBoth<string, int>.Both("left", 1).Match(left: Identity, right: static r => r.ToString(), both: static (l, r) => l + r));
    }

    [Fact]
    public void EqualValuesHaveEqualHashCodes()
    {
        Assert.Equal(EitherOrBoth<string, int>.Both("a", 1), EitherOrBoth<string, int>.Both("a", 1));
        Assert.Equal(EitherOrBoth<string, int>.Both("a", 1).GetHashCode(), EitherOrBoth<string, int>.Both("a", 1).GetHashCode());
        Assert.NotEqual(EitherOrBoth<string, int>.Left("a"), EitherOrBoth<string, int>.Both("a", 1));
    }

    [Fact]
    [SuppressMessage("Funcky", "λ1009:Do not use default to instantiate this type", Justification = "Intentionally creating an invalid instance.")]
    public void MatchAndSwitchThrowWhenCreatedWithDefault()
    {
        var value = default(EitherOrBoth<string, int>);

        Assert.Throws<NotSupportedException>(() => value.Match(left: Identity, right: static r => r.ToString(), both: static (l, r) => l + r));
        Assert.Throws<NotSupportedException>(() => value.Switch(left: NoOperation, right: NoOperation, both: NoOperation));
    }

    [Fact]
    [SuppressMessage("Funcky", "λ1009:Do not use default to instantiate this type", Justification = "Intentionally creating invalid instances.")]
    public void EqualsGetHashCodeAndToStringAreTotalForADefaultValue()
    {
        var value = default(EitherOrBoth<string, int>);

        Assert.Equal(default, value);
        Assert.NotEqual(EitherOrBoth<string, int>.Left("left"), value);
        Assert.Equal(default(EitherOrBoth<string, int>).GetHashCode(), value.GetHashCode());
        Assert.NotEmpty($"{value}");
        Assert.Contains(default, new HashSet<EitherOrBoth<string, int>> { default });
    }
}
