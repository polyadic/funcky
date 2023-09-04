#if GENERIC_PARSEABLE
using System.Globalization;

namespace Funcky.Test.Extensions.ParseExtensions;

public sealed partial class ParseExtensionsTest
{
    [Theory]
    [MemberData(nameof(ParseableDoubleNumbers))]
    public void ParseGenericStringReturnsTheExpectedDouble(Option<double> expected, string input)
    {
        Assert.Equal(expected, input.ParseNumberOrNone<double>(NumberStyles.Number, null));
        Assert.Equal(expected, input.ParseOrNone<double>(null));
    }

    [Theory]
    [MemberData(nameof(ParseableDoubleNumbers))]
    public void ParseGenericSpanReturnsTheExpectedDouble(Option<double> expected, string input)
    {
        Assert.Equal(expected, input.AsSpan().ParseNumberOrNone<double>(NumberStyles.Number, null));
        Assert.Equal(expected, input.AsSpan().ParseOrNone<double>(null));
    }

    public static TheoryData<Option<double>, string> ParseableDoubleNumbers()
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
