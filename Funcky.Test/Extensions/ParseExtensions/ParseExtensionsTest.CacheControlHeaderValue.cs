namespace Funcky.Test.Extensions.ParseExtensions;

public sealed partial class ParseExtensionsTest
{
    /// <summary>Backport of https://github.com/dotnet/runtime/pull/74863.</summary>
    [Fact]
    public void ReturnsEmptyValueForEmptyString()
    {
        var value = FunctionalAssert.Some(string.Empty.ParseCacheControlHeaderValueOrNone());
        Assert.Equal(string.Empty, value.ToString());
    }
}
