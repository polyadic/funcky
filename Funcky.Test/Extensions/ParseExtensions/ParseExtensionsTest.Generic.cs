#pragma warning disable λ1201 // This test exercises the classic string extension form itself.

#if GENERIC_PARSABLE
using System.Globalization;

namespace Funcky.Test.Extensions.ParseExtensions;

public sealed partial class ParseExtensionsTest
{
    [Theory]
    [MemberData(nameof(ParsableDoubleNumbers))]
    public void ParseGenericStringReturnsTheExpectedDouble(Option<double> expected, string input)
    {
        Assert.Equal(expected, input.ParseNumberOrNone<double>(NumberStyles.Number, null));
        Assert.Equal(expected, input.ParseOrNone<double>(null));
    }

    [Theory]
    [MemberData(nameof(ParsableDoubleNumbers))]
    public void ParseGenericSpanReturnsTheExpectedDouble(Option<double> expected, string input)
    {
        Assert.Equal(expected, input.AsSpan().ParseNumberOrNone<double>(NumberStyles.Number, null));
        Assert.Equal(expected, input.AsSpan().ParseOrNone<double>(null));
    }

    [Theory]
    [MemberData(nameof(ParsableDoubleNumbers))]
    public void ParseGenericWithoutProviderUsesTheCurrentCulture(Option<double> expected, string input)
    {
        Assert.Equal(expected, input.ParseOrNone<double>());
        Assert.Equal(expected, input.AsSpan().ParseOrNone<double>());
        Assert.Equal(expected, input.ParseNumberOrNone<double>());
        Assert.Equal(expected, input.AsSpan().ParseNumberOrNone<double>());
        Assert.Equal(expected, input.ParseNumberOrNone<double>(CultureInfo.InvariantCulture));
        Assert.Equal(expected, input.AsSpan().ParseNumberOrNone<double>(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseNumberWithoutStyleUsesTheDefaultStyleOfTheType()
    {
        Assert.Equal(Option.Some(42), "42".ParseNumberOrNone<int>());
        Assert.Equal(Option.Some(-42), " -42 ".ParseNumberOrNone<int>());
        Assert.Equal(Option<int>.None, "4.2".ParseNumberOrNone<int>());
        Assert.Equal(Option<int>.None, "1,000".ParseNumberOrNone<int>(CultureInfo.InvariantCulture));
        Assert.Equal(Option.Some(1000.0), "1,000".ParseNumberOrNone<double>(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseNumberOrNoneReturnsNoneForNull()
    {
        string? candidate = null;

        Assert.Equal(Option<int>.None, candidate.ParseNumberOrNone<int>());
        Assert.Equal(Option<int>.None, candidate.ParseNumberOrNone<int>(null));
        Assert.Equal(Option<int>.None, candidate.ParseNumberOrNone<int>(NumberStyles.Integer, null));
        Assert.Equal(Option<int>.None, candidate.ParseOrNone<int>());
    }

#if UTF8_SPAN_PARSABLE
    [Fact]
    public void ParseGenericUtf8SpanWithoutProviderParses()
    {
        Assert.Equal(Option.Some(42), "42"u8.ParseOrNone<int>());
        Assert.Equal(Option<int>.None, "no"u8.ParseOrNone<int>());
    }
#endif

    public static TheoryData<Option<double>, string> ParsableDoubleNumbers()
        => new()
        {
            { Option.Some(1.0), "1.0" },
            { Option.Some(3.145), "3.145" },
            { Option.Some(0.5), ".5" },
            { Option.Some(1.0), "1.0" },
            { Option.Some(42.0), "42" },
            { Option<double>.None, string.Empty },
            { Option<double>.None, "no-number" },
        };
}
#endif
