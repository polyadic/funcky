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

    [Theory]
    [InlineData("1")]
    [InlineData("")]
    [InlineData("invalid-version")]
    public void ParseVersionIsTheSameAsTryParseForInvalidVersions(string input)
    {
        Assert.False(Version.TryParse(input, out _));
        FunctionalAssert.IsNone(input.ParseVersionOrNone());
    }

    #if PARSE_READ_ONLY_SPAN_SUPPORTED
    [Theory]
    [InlineData("1")]
    [InlineData("")]
    [InlineData("invalid-version")]
    public void ParseVersionIsTheSameAsTryParseForInvalidVersionsWithReadOnlySpan(string input)
    {
        var inputSpan = input.AsSpan();
        Assert.False(Version.TryParse(inputSpan, out _));
        FunctionalAssert.IsNone(inputSpan.ParseVersionOrNone());
    }
    #endif
}
