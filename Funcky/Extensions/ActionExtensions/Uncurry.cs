namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2> Uncurry<T1, T2>(this Func<T1, Action<T2>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3> Uncurry<T1, T2, T3>(this Func<T1, Func<T2, Action<T3>>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3, T4> Uncurry<T1, T2, T3, T4>(this Func<T1, Func<T2, Func<T3, Action<T4>>>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3, T4, T5> Uncurry<T1, T2, T3, T4, T5>(this Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6> Uncurry<T1, T2, T3, T4, T5, T6>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7> Uncurry<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> action)
        => Functional.Uncurry(action);

    /// <summary>
    /// Transforms a curried action into an action that takes multiple arguments.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Uncurry<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> action)
        => Functional.Uncurry(action);
}
