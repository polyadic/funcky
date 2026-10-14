using System.Diagnostics.CodeAnalysis;

namespace Funcky.Test.Monads;

[SuppressMessage("Funcky", "λ1009:Do not use default to instantiate this type", Justification = "Intentionally creating invalid instances.")]
public sealed partial class EitherTest
{
    [Fact]
    public void SwitchThrowsWhenEitherIsCreatedWithDefault()
    {
        var value = default(Either<string, int>);

        Assert.Throws<NotSupportedException>(() => value.Switch(left: NoOperation, right: NoOperation));
    }

    [Fact]
    public void DefaultEithersAreEqualToEachOtherButNotToLeftOrRight()
    {
        var value = default(Either<string, int>);

        Assert.Equal(default, value);
        Assert.NotEqual(Either<string, int>.Left("left"), value);
        Assert.NotEqual(Either<string, int>.Right(42), value);
    }

    [Fact]
    public void GetHashCodeIsTotalForADefaultEither()
    {
        var value = default(Either<string, int>);

        Assert.Equal(default(Either<string, int>).GetHashCode(), value.GetHashCode());
    }

    [Fact]
    public void ADefaultEitherCanBeStoredInAHashSet()
    {
        var set = new HashSet<Either<string, int>> { default, Either<string, int>.Right(1) };

        Assert.Contains(default, set);
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void ToStringIsTotalForADefaultEither()
    {
        var value = default(Either<string, int>);

        Assert.NotEmpty($"{value}");
    }
}
