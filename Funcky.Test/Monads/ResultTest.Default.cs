using System.Diagnostics.CodeAnalysis;

namespace Funcky.Test.Monads;

[SuppressMessage("Funcky", "λ1009:Do not use default to instantiate this type", Justification = "Intentionally creating invalid instances.")]
public sealed partial class ResultTest
{
    [Fact]
    public void MatchThrowsWhenResultIsCreatedWithDefault()
    {
        var value = default(Result<string>);

        Assert.Throws<NotSupportedException>(() => value.Match(ok: static s => s.ToUpperInvariant(), error: static exception => exception.Message));
    }

    [Fact]
    public void MatchThrowsWhenResultOfAValueTypeIsCreatedWithDefault()
    {
        var value = default(Result<int>);

        Assert.Throws<NotSupportedException>(() => value.Match(ok: static n => n + 1, error: static _ => -1));
    }

    [Fact]
    public void SwitchThrowsWhenResultIsCreatedWithDefault()
    {
        var value = default(Result<string>);

        Assert.Throws<NotSupportedException>(() => value.Switch(ok: NoOperation, error: NoOperation));
    }

    [Fact]
    public void SelectThrowsWhenResultIsCreatedWithDefault()
    {
        var value = default(Result<string>);

        Assert.Throws<NotSupportedException>(() => value.Select(static s => s.Length));
    }

    [Fact]
    public void DefaultResultsAreEqualToEachOtherButNotToOkOrError()
    {
        var value = default(Result<string>);

        Assert.Equal(default, value);
        Assert.NotEqual(Result.Ok("ok"), value);
        Assert.NotEqual(Result<string>.Error(new InvalidOperationException()), value);
    }

    [Fact]
    public void GetHashCodeAndToStringAreTotalForADefaultResult()
    {
        var value = default(Result<string>);

        Assert.Equal(default(Result<string>).GetHashCode(), value.GetHashCode());
        Assert.NotEmpty(value.ToString());
    }
}
