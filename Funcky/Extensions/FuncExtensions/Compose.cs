namespace Funcky.Extensions
{
    public static partial class FuncExtensions
    {
        /// <summary>
        /// Function composition.
        /// </summary>
        /// <returns>f ∘ g, v => f(g(v)).</returns>
        [Pure]
        public static Func<TInput, TOutput> Compose<TInput, TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TInput, TIntermediate> g)
            => v => f(g(v));

        /// <summary>
        /// Function composition.
        /// </summary>
        /// <returns>f ∘ g,() => f(g()).</returns>
        [Pure]
        public static Func<TOutput> Compose<TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TIntermediate> g)
            => () => f(g());

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
}
