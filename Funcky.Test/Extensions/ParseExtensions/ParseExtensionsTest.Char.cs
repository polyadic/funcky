namespace Funcky.Test.Extensions.ParseExtensions;

public sealed partial class ParseExtensionsTest
{
    [Theory]
    [InlineData('a', "a")]
    [InlineData('x', "x")]
    [InlineData('1', "1")]
    [InlineData('δ', "δ")]
    public void ParseCharOrNoneReturnsTheOnlyCharacterInAString(char expected, string input)
    {
        FunctionalAssert.Some(expected, input.ParseCharOrNone());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("longer")]
    [InlineData("")]
    [InlineData("\ud83d\udd25")] // single fire emoji (outside BMP)
    public void ParseCharOrNoneReturnsNoneIfItCanParseItToACharcter(string? input)
    {
        FunctionalAssert.None(input.ParseCharOrNone());
    }
}
