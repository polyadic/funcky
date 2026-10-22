namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<TInput, TIntermediate>(Func<TInput, TIntermediate> f)
    {
        /// <summary>
        /// Forward function composition: applies this function first and <paramref name="next"/> to its result.
        /// </summary>
        /// <remarks><c>f.Then(g)</c> is <c>g.Compose(f)</c>; the forward form reads in the order of evaluation.</remarks>
        /// <returns>g ∘ f, v => next(f(v)).</returns>
        /// <seealso cref="Compose{TInput, TIntermediate, TOutput}"/>
        [Pure]
        public Func<TInput, TOutput> Then<TOutput>(Func<TIntermediate, TOutput> next)
            => v => next(f(v));

        /// <summary>
        /// Forward function composition with an action: applies this function first and <paramref name="next"/> to its result.
        /// </summary>
        /// <returns>g ∘ f, v => next(f(v)).</returns>
        /// <seealso cref="ActionExtensions.Compose{TInput, TIntermediate}"/>
        [Pure]
        public Action<TInput> Then(Action<TIntermediate> next)
            => v => next(f(v));
    }

    extension<TIntermediate>(Func<TIntermediate> f)
    {
        /// <summary>
        /// Forward function composition: applies this function first and <paramref name="next"/> to its result.
        /// </summary>
        /// <returns>g ∘ f, () => next(f()).</returns>
        /// <seealso cref="Compose{TIntermediate, TOutput}"/>
        [Pure]
        public Func<TOutput> Then<TOutput>(Func<TIntermediate, TOutput> next)
            => () => next(f());

        /// <summary>
        /// Forward function composition with an action: applies this function first and <paramref name="next"/> to its result.
        /// </summary>
        /// <returns>g ∘ f, () => next(f()).</returns>
        /// <seealso cref="ActionExtensions.Compose{TIntermediate}"/>
        [Pure]
        public Action Then(Action<TIntermediate> next)
            => () => next(f());
    }
}
