using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.ParseExtensions;

public sealed class ParseExtensionsTest
{
    [Theory]
    [InlineData("1.0")]
    [InlineData("1.0.0")]
    [InlineData("1.0.0.0")]
    public void ParseVersionIsTheSameAsTryParseForValidVersions(string input)
    {
        Assert.True(Version.TryParse(input, out var expected));
        FunctionalAssert.IsSome(expected!, input.ParseVersionOrNone());
    }

    #if PARSE_READ_ONLY_SPAN_SUPPORTED
    [Theory]
    [InlineData("1.0")]
    [InlineData("1.0.0")]
    [InlineData("1.0.0.0")]
    public void ParseVersionIsTheSameAsTryParseForValidVersionsWithReadOnlySpan(string input)
    {
        var inputSpan = input.AsSpan();
        Assert.True(Version.TryParse(inputSpan, out var expected));
        FunctionalAssert.IsSome(expected!, inputSpan.ParseVersionOrNone());
    }
    #endif

    [Property]
    public Property ParseVersionIsTheSameAsTryParse(string input)
    {
        var parsed = input.ParseVersionOrNone();
        var result = Version.TryParse(input, out var expected)
            ? parsed.Match(none: false, some: version => version == expected)
            : parsed.Match(none: true, some: False);
        return result.ToProperty();
    }

    #if PARSE_READ_ONLY_SPAN_SUPPORTED
    [Property]
    public Property ParseVersionIsTheSameAsTryParseWithReadOnlySpan(string input)
    {
        var inputSpan = input.AsSpan();
        var parsed = inputSpan.ParseVersionOrNone();
        var result = Version.TryParse(inputSpan, out var expected)
            ? parsed.Match(none: false, some: version => version == expected)
            : parsed.Match(none: true, some: False);
        return result.ToProperty();
    }
    #endif
}
