namespace Funcky.Test.Extensions;

internal sealed class SideEffect
{
    private string _string;

    public SideEffect()
        => _string = string.Empty;

    public void Store(string s)
        => _string = s;

    public string Retrieve()
        => _string;
}
