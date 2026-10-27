namespace Funcky.Test.Extensions;

public sealed class HttpHeadersExtensionsTest
{
    [Fact]
    public void LocationOrNoneReturnsTheLocationHeader()
    {
        using var response = new System.Net.Http.HttpResponseMessage();
        var location = new Uri("https://example.com/moved");
        response.Headers.Location = location;

        FunctionalAssert.Some(location, response.Headers.LocationOrNone);
    }

    [Fact]
    public void LocationOrNoneReturnsNoneWithoutALocationHeader()
    {
        using var response = new System.Net.Http.HttpResponseMessage();

        FunctionalAssert.None(response.Headers.LocationOrNone);
    }
}
