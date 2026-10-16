namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(this Func<T1, T2, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, TResult> Flip<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, T4, TResult> Flip<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, TResult> Flip<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, TResult> Flip<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, T7, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        => Functional.Flip(function);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, T7, T8, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        => Functional.Flip(function);
}
