namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Action<T2>> Curry<T1, T2>(this Action<T1, T2> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Action<T3>>> Curry<T1, T2, T3>(this Action<T1, T2, T3> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> Curry<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> Curry<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action)
        => Functional.Curry(action);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => Functional.Curry(action);
}
