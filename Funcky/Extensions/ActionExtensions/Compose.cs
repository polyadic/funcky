namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    /// <summary>
    /// Function composition with action.
    /// </summary>
    /// <returns>f ∘ g,v => f(g(v)).</returns>
    [Pure]
    public static Action<TInput> Compose<TInput, TIntermediate>(this Action<TIntermediate> f, Func<TInput, TIntermediate> g)
        => v => f(g(v));

    /// <summary>
    /// Function composition with Action.
    /// </summary>
    /// <returns>f ∘ g, () => f(g()).</returns>
    [Pure]
    public static Action Compose<TIntermediate>(this Action<TIntermediate> f, Func<TIntermediate> g)
        => () => f(g());
}
