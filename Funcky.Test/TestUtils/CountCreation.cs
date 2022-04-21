namespace Funcky.Test.TestUtils;

internal sealed class CountCreation
{
    public CountCreation()
    {
        Count += 1;
    }

    public static int Count { get; private set; }
}
