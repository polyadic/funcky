using System.Runtime.CompilerServices;

namespace Funcky;

public static partial class Functional
{
    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <example>
    /// Flipping a method group using <see cref="Fn{T}"/> to help with inference:
    /// <code><![CDATA[
    /// var flippedPow = Flip(Fn(Math.Pow));
    /// ]]></code>
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(Func<T1, T2, TResult> function)
        => (p1, p2) => function(p2, p1);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, TResult> Flip<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        => (p1, p2, p3) => function(p2, p1, p3);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, T4, TResult> Flip<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
        => (p1, p2, p3, p4) => function(p2, p1, p3, p4);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, T4, T5, TResult> Flip<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function)
        => (p1, p2, p3, p4, p5) => function(p2, p1, p3, p4, p5);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, T4, T5, T6, TResult> Flip<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function)
        => (p1, p2, p3, p4, p5, p6) => function(p2, p1, p3, p4, p5, p6);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, T4, T5, T6, T7, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        => (p1, p2, p3, p4, p5, p6, p7) => function(p2, p1, p3, p4, p5, p6, p7);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T2, T1, T3, T4, T5, T6, T7, T8, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => function(p2, p1, p3, p4, p5, p6, p7, p8);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1> Flip<T1, T2>(Action<T1, T2> function)
        => (p1, p2) => function(p2, p1);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3> Flip<T1, T2, T3>(Action<T1, T2, T3> action)
        => (p1, p2, p3) => action(p2, p1, p3);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3, T4> Flip<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        => (p1, p2, p3, p4) => action(p2, p1, p3, p4);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3, T4, T5> Flip<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
        => (p1, p2, p3, p4, p5) => action(p2, p1, p3, p4, p5);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3, T4, T5, T6> Flip<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
        => (p1, p2, p3, p4, p5, p6) => action(p2, p1, p3, p4, p5, p6);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3, T4, T5, T6, T7> Flip<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
        => (p1, p2, p3, p4, p5, p6, p7) => action(p2, p1, p3, p4, p5, p6, p7);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Action<T2, T1, T3, T4, T5, T6, T7, T8> Flip<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => action(p2, p1, p3, p4, p5, p6, p7, p8);
}
