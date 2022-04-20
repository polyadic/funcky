namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(this Func<T1, T2, TResult> function)
        => (p1, p2) => function(p2, p1);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, TResult> Flip<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function)
        => (p1, p2, p3) => function(p2, p1, p3);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, T4, TResult> Flip<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function)
        => (p1, p2, p3, p4) => function(p2, p1, p3, p4);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, TResult> Flip<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> function)
        => (p1, p2, p3, p4, p5) => function(p2, p1, p3, p4, p5);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, TResult> Flip<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> function)
        => (p1, p2, p3, p4, p5, p6) => function(p2, p1, p3, p4, p5, p6);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, T7, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        => (p1, p2, p3, p4, p5, p6, p7) => function(p2, p1, p3, p4, p5, p6, p7);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Func<T2, T1, T3, T4, T5, T6, T7, T8, TResult> Flip<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => function(p2, p1, p3, p4, p5, p6, p7, p8);
}
