namespace Funcky.Test.Extensions;

public class NumberExtensionsTest
{
    [Fact]
    public void Example()
    {
        var position = 12;

        Assert.True(position.IsBetween<Including, Excluding>(20, 0));
    }
}
