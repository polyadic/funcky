namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3> Flip<T1, T2, T3>(this Action<T1, T2, T3> action)
        => Functional.Flip(action);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3, T4> Flip<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        => Functional.Flip(action);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3, T4, T5> Flip<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        => Functional.Flip(action);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6> Flip<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        => Functional.Flip(action);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6, T7> Flip<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action)
        => Functional.Flip(action);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6, T7, T8> Flip<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => Functional.Flip(action);
}
