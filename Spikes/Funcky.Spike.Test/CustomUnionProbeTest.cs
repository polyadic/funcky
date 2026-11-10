using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Funcky.Spike.Test;

/// <summary>Day-2 probes: hiding <c>Value</c> behind a union member provider, and what System.Text.Json does with a
/// custom union that has no converter.</summary>
public sealed class CustomUnionProbeTest
{
    [Fact]
    public void ProviderInterfaceHidesValueKeepsConstructorsPrivateAndStillDrivesPatterns()
    {
        // CustomUnionResult declares a nested IUnionMembers provider: `result.Value` is not a public member of the
        // struct (explicit implementation), the constructors are private, and the cases come from the provider's
        // static Create methods. Patterns, exhaustiveness and the non-boxing TryGetValue (declared on the provider) work.
        var ok = CustomUnionResult<int>.Ok(5);
        Assert.True(ok is int);
        Assert.Equal("ok", ok switch { int => "ok", Exception => "error" });
        Assert.Null(((IUnion)default(CustomUnionResult<int>)).Value);
        Assert.Empty(typeof(CustomUnionResult<int>).GetConstructors());
        Assert.Null(typeof(CustomUnionResult<int>).GetProperty("Value"));
    }

    [Fact]
    public void UnionConversionsGiveTheSimplerConstructorsOf809ForFree()
    {
        // A union conversion exists from every case type (spec), through the constructor or the provider's Create:
        // no implicit operator is declared for these in the spike.
        CustomUnionEither<string, int> left = "l";
        CustomUnionEither<string, int> right = 5;
        CustomUnionResult<int> error = new InvalidOperationException("x");
        CustomUnionOption<int> none = default(None);
        Assert.True(left is string);
        Assert.True(right is int);
        Assert.True(error is InvalidOperationException);
        Assert.True(none is None);
        // Either<T, T> has no unambiguous union conversion (both creation members apply): a compile error, use Left/Right.
    }

    [Fact]
    public void ATypePatternOnTheCaseIsADownCast()
    {
        // #868: `option is Derived d` applies the pattern to the case, so DownCast reads as a pattern.
        CustomUnionOption<object> option = CustomUnionOption.Some<object>("x");
        Assert.True(option is string { Length: 1 });
        Assert.False(option is int);
        // Caveat for T = object: the compiler treats the `object` case as covering everything (a None arm after it is
        // CS8510, CS8655 warns about null), and `is object` lowers to a null check (HasValue), not to TryGetValue(out object):
        // a None IS `object` at runtime. Option<object> is the one instantiation where the case pattern lies.
#pragma warning disable CS8655
        Assert.Equal(1, option switch { string s => s.Length, None => 0, object => -1 });
#pragma warning restore CS8655
        Assert.True(CustomUnionOption<object>.None is object);
    }

    [Fact]
    public void SystemTextJsonNeedsAConverterForEveryCustomUnion()
    {
        // .NET 11's System.Text.Json recognizes IUnion and builds a union contract from the public single-parameter
        // constructors. With the provider shape there are none: it throws instead of falling back to the struct contract
        // (which today writes `{}` for Either and Result). Funcky has to ship converters for all three types.
        var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(CustomUnionResult<int>.Ok(5)));
        Assert.Contains("must specify at least one union case", exception.Message);

        // With public constructors (Option, Either in the spike) the contract exists and writes the case itself.
        Assert.Equal("5", JsonSerializer.Serialize(CustomUnionOption.Some(5)));
        Assert.Equal("{}", JsonSerializer.Serialize(CustomUnionOption<int>.None));
        Assert.Equal("5", JsonSerializer.Serialize(CustomUnionEither<string, int>.Right(5)));
        Assert.Equal("\"l\"", JsonSerializer.Serialize(CustomUnionEither<string, int>.Left("l")));
    }
}
