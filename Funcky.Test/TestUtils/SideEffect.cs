namespace Funcky.Test.TestUtils;

internal sealed class SideEffect
{
    public bool IsDone { get; private set; }

    public void Do() => IsDone = true;
}
