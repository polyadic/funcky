namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    extension<TIntermediate>(Action<TIntermediate> f)
    {
        /// <summary>
        /// Function composition with action.
        /// </summary>
        /// <returns>f ∘ g,v => f(g(v)).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<TInput> Compose<TInput>(Func<TInput, TIntermediate> g)
            => v => f(g(v));

        /// <summary>
        /// Function composition with Action.
        /// </summary>
        /// <returns>f ∘ g, () => f(g()).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action Compose(Func<TIntermediate> g)
            => () => f(g());
    }
}
