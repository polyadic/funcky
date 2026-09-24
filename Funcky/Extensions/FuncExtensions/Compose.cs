namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    /// <summary>
    /// Function composition.
    /// </summary>
    /// <returns>f ∘ g, v => f(g(v)).</returns>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<TInput, TOutput> Compose<TInput, TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TInput, TIntermediate> g)
        => v => f(g(v));

    /// <summary>
    /// Function composition.
    /// </summary>
    /// <returns>f ∘ g,() => f(g()).</returns>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<TOutput> Compose<TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TIntermediate> g)
        => () => f(g());
}
