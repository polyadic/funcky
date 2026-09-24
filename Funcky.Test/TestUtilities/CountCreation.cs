namespace Funcky.Test.TestUtilities;

internal sealed class CountCreation
{
    public CountCreation()
    {
        Count += 1;
    }

    public static int Count { get; private set; }
}
