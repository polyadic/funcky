namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    extension<TIntermediate>(Action<TIntermediate> f)
    {
        /// <summary>
        /// Function composition with Action.
        /// </summary>
        /// <returns>f ∘ g, () => f(g()).</returns>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action Compose(Func<TIntermediate> g)
            => () => f(g());
    }

    /// <summary>
    /// Function composition with action.
    /// </summary>
    /// <returns>f ∘ g,v => f(g(v)).</returns>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<TInput> Compose<TInput, TIntermediate>(this Action<TIntermediate> f, Func<TInput, TIntermediate> g)
        => v => f(g(v));
}
