using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Funcky.Spike.Test;

public sealed class UnionOptionTest
{
    [Fact]
    public void ImplicitConversionFromTheValueAndFromTheCaseBothWork()
    {
        UnionOption<int> fromValue = 5;
        UnionOption<int> fromCase = new Some<int>(5);
        Assert.Equal(fromValue, fromCase);
        Assert.Equal("Some(5)", fromValue.ToString());
    }

    [Fact]
    public void IsPatternsOnTheCaseTypesWork()
    {
        UnionOption<int> option = 5;
        Assert.True(option is Some<int>);
        Assert.True(option is Some<int>(5));
        Assert.True(option is Some<int>(var value) && value == 5);
        Assert.False(option is None);
        Assert.True(UnionOption<int>.Empty is None);
    }

    [Fact]
    public void ListPatternsAreGone()
    {
        // Option<T> today: `option is [var value]`. A union has no Count/indexer and cannot get them through the union
        // body (it is a struct with members, so it could, but the pattern would then match the union, not its case).
        // Documented as a breaking call site for the upgrade chapter.
        UnionOption<int> option = 5;
        Assert.True(option.TryGetValue(out var value) && value == 5);
    }

    [Fact]
    public void DefaultIsNeitherSomeNorNoneAndAnExhaustiveSwitchThrowsAtRuntime()
    {
        UnionOption<int> option = default;
        Assert.False(option is Some<int>);
        Assert.False(option is None);
        Assert.Null(option.Value);

        // The compiler considers Some + None exhaustive for the cases (no CS8509), but it knows about the null union:
        // CS8655 "does not handle some null inputs" is the only hint a consumer gets, and it is a warning.
#pragma warning disable CS8655
        Assert.Throws<SwitchExpressionException>(() => option switch
        {
            Some<int> => "some",
            None => "none",
        });
#pragma warning restore CS8655

        // The spike's own Match uses a catch-all and reports default as None.
        Assert.Equal("none", option.Match(none: "none", some: _ => "some"));
    }

    [Fact]
    public void EverySomeBoxesItsCase()
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        UnionOption<int> option = default(None);
        for (var i = 0; i < 1000; i++)
        {
            option = i;
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        // Each Some<int> (4 bytes payload) becomes a 24-byte heap object (header + method table + 8 byte aligned payload).
        Assert.InRange(allocated, 24 * 1000, 24 * 1000 + 1024);
        Assert.True(option.IsSome);
    }

    [Fact]
    public void SelectAndSelectManyFollowTheMonadLaws()
    {
        UnionOption<int> some = 21;
        Assert.Equal(UnionOption.Some(42), some.Select(x => x * 2));
        Assert.Equal(UnionOption.Some(42), some.SelectMany(x => UnionOption.Some(x * 2)));
        Assert.Equal(UnionOption<int>.Empty, UnionOption<int>.Empty.Select(x => x * 2));
        Assert.Equal(UnionOption.Some("21"), from x in some select x.ToString());
        Assert.Equal(UnionOption.Some(42), from x in some from y in UnionOption.Some(x) select x + y);
    }

    [Fact]
    public void JsonWithoutAConverterSerializesTheCaseItself()
    {
        // System.Text.Json in .NET 11 knows IUnion: it writes the current case, not the generated `Value` wrapper.
        // The case record `Some<int>` still shows up as an object with its own Value property.
        UnionOption<int> option = 5;
        Assert.Equal("""{"Value":5}""", JsonSerializer.Serialize(option));
        Assert.Equal("{}", JsonSerializer.Serialize(UnionOption<int>.Empty));
    }

    [Fact]
    public void JsonWithTheConverterMatchesTodaysContract()
    {
        var options = new JsonSerializerOptions { Converters = { new UnionOptionJsonConverter() } };
        UnionOption<int> option = 5;
        Assert.Equal("5", JsonSerializer.Serialize(option, options));
        Assert.Equal("null", JsonSerializer.Serialize(UnionOption<int>.Empty, options));
        Assert.Equal(option, JsonSerializer.Deserialize<UnionOption<int>>("5", options));
        Assert.Equal(UnionOption<int>.Empty, JsonSerializer.Deserialize<UnionOption<int>>("null", options));
    }

    [Fact]
    public void BareOptionNeedsNoWrapperButCannotNestANone()
    {
        BareOption<int> option = 5;
        Assert.Equal("Some(5)", option.ToString());
        Assert.Equal("Some(6)", option.Select(x => x + 1).ToString());

        // BareOption<None> cannot even be constructed: both generated conversions take a None (CS0457, ambiguous).
        // Some(None) and None collapse, the Nullable<T> flattening Funcky's Option exists to avoid.
        Assert.Equal(2, typeof(BareOption<None>).GetConstructors().Length);
    }

    [Fact]
    public void EitherAndResultWorkAsUnionsToo()
    {
        UnionEither<string, int> right = new Right<int>(1);
        Assert.Equal("Right(1)", right.ToString());
        Assert.Equal("Right(2)", right.Select(x => x + 1).ToString());
        Assert.Equal("Left(err)", new UnionEither<string, int>(new Left<string>("err")).Select(x => x + 1).ToString());

        UnionResult<int> ok = 1;
        UnionResult<int> error = new InvalidOperationException();
        Assert.True(ok is Ok<int>(1));
        Assert.True(error is InvalidOperationException);
        Assert.Equal("Error(InvalidOperationException)", error.Select(x => x + 1).ToString());
    }
}
