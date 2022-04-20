namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> function)
        => (p1, p2) => function(p2, p1);

    /// <summary>
    /// Flips the first two arguments of the function.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3> Flip<T1, T2, T3>(this Action<T1, T2, T3> action)
        => (p1, p2, p3) => action(p2, p1, p3);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3, T4> Flip<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        => (p1, p2, p3, p4) => action(p2, p1, p3, p4);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3, T4, T5> Flip<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        => (p1, p2, p3, p4, p5) => action(p2, p1, p3, p4, p5);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6> Flip<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        => (p1, p2, p3, p4, p5, p6) => action(p2, p1, p3, p4, p5, p6);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6, T7> Flip<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action)
        => (p1, p2, p3, p4, p5, p6, p7) => action(p2, p1, p3, p4, p5, p6, p7);

    /// <summary>
    /// Flips the first two arguments of the action.
    /// </summary>
    [Pure]
    public static Action<T2, T1, T3, T4, T5, T6, T7, T8> Flip<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => action(p2, p1, p3, p4, p5, p6, p7, p8);
}
