namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<TIntermediate, TOutput>(Func<TIntermediate, TOutput> f)
    {
        /// <summary>
        /// Function composition.
        /// </summary>
        /// <returns>f ∘ g, v => f(g(v)).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<TInput, TOutput> Compose<TInput>(Func<TInput, TIntermediate> g)
            => v => f(g(v));

        /// <summary>
        /// Function composition.
        /// </summary>
        /// <returns>f ∘ g,() => f(g()).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<TOutput> Compose(Func<TIntermediate> g)
            => () => f(g());
    }
}
