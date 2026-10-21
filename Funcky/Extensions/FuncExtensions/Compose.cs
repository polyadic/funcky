namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<TIntermediate, TOutput>(Func<TIntermediate, TOutput> f)
    {
        /// <summary>
        /// Function composition.
        /// </summary>
        /// <returns>f ∘ g,() => f(g()).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<TOutput> Compose(Func<TIntermediate> g)
            => () => f(g());
    }

    /// <summary>
    /// Function composition.
    /// </summary>
    /// <returns>f ∘ g, v => f(g(v)).</returns>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<TInput, TOutput> Compose<TInput, TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TInput, TIntermediate> g)
        => v => f(g(v));
}
