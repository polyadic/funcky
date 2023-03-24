namespace Funcky.Fluent;

public static class FluentExtensions
{
    public static TFluent ExecuteFluent<TFluent>(this TFluent nonFluent, Action<TFluent> action)
    {
        action(nonFluent);

        return nonFluent;
    }
}
