namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    /// <summary>
    /// Forward function composition: applies this function first and <paramref name="next"/> to its result.
    /// </summary>
    /// <remarks><c>f.Then(g)</c> is <c>g.Compose(f)</c>; the forward form reads in the order of evaluation.</remarks>
    /// <returns>g ∘ f, v => next(f(v)).</returns>
    /// <seealso cref="Compose{TInput, TIntermediate, TOutput}"/>
    [Pure]
    public static Func<TInput, TOutput> Then<TInput, TIntermediate, TOutput>(this Func<TInput, TIntermediate> f, Func<TIntermediate, TOutput> next)
        => v => next(f(v));

    /// <summary>
    /// Forward function composition: applies this function first and <paramref name="next"/> to its result.
    /// </summary>
    /// <returns>g ∘ f, () => next(f()).</returns>
    /// <seealso cref="Compose{TIntermediate, TOutput}"/>
    [Pure]
    public static Func<TOutput> Then<TIntermediate, TOutput>(this Func<TIntermediate> f, Func<TIntermediate, TOutput> next)
        => () => next(f());

    /// <summary>
    /// Forward function composition with an action: applies this function first and <paramref name="next"/> to its result.
    /// </summary>
    /// <returns>g ∘ f, v => next(f(v)).</returns>
    /// <seealso cref="ActionExtensions.Compose{TInput, TIntermediate}"/>
    [Pure]
    public static Action<TInput> Then<TInput, TIntermediate>(this Func<TInput, TIntermediate> f, Action<TIntermediate> next)
        => v => next(f(v));

    /// <summary>
    /// Forward function composition with an action: applies this function first and <paramref name="next"/> to its result.
    /// </summary>
    /// <returns>g ∘ f, () => next(f()).</returns>
    /// <seealso cref="ActionExtensions.Compose{TIntermediate}"/>
    [Pure]
    public static Action Then<TIntermediate>(this Func<TIntermediate> f, Action<TIntermediate> next)
        => () => next(f());
}
