namespace Funcky;

public static partial class Functional
{
    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> function)
        => (p1, p2) => function(p1)(p2);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, TResult> Uncurry<T1, T2, T3, TResult>(Func<T1, Func<T2, Func<T3, TResult>>> function)
        => (p1, p2, p3) => function(p1)(p2)(p3);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, TResult> Uncurry<T1, T2, T3, T4, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> function)
        => (p1, p2, p3, p4) => function(p1)(p2)(p3)(p4);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, TResult> Uncurry<T1, T2, T3, T4, T5, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> function)
        => (p1, p2, p3, p4, p5) => function(p1)(p2)(p3)(p4)(p5);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, TResult> Uncurry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> function)
        => (p1, p2, p3, p4, p5, p6) => function(p1)(p2)(p3)(p4)(p5)(p6);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Uncurry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> function)
        => (p1, p2, p3, p4, p5, p6, p7) => function(p1)(p2)(p3)(p4)(p5)(p6)(p7);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Uncurry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> function)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => function(p1)(p2)(p3)(p4)(p5)(p6)(p7)(p8);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2> Uncurry<T1, T2>(Func<T1, Action<T2>> action)
        => (p1, p2) => action(p1)(p2);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3> Uncurry<T1, T2, T3>(Func<T1, Func<T2, Action<T3>>> action)
        => (p1, p2, p3) => action(p1)(p2)(p3);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3, T4> Uncurry<T1, T2, T3, T4>(Func<T1, Func<T2, Func<T3, Action<T4>>>> action)
        => (p1, p2, p3, p4) => action(p1)(p2)(p3)(p4);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3, T4, T5> Uncurry<T1, T2, T3, T4, T5>(Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> action)
        => (p1, p2, p3, p4, p5) => action(p1)(p2)(p3)(p4)(p5);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6> Uncurry<T1, T2, T3, T4, T5, T6>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> action)
        => (p1, p2, p3, p4, p5, p6) => action(p1)(p2)(p3)(p4)(p5)(p6);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7> Uncurry<T1, T2, T3, T4, T5, T6, T7>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> action)
        => (p1, p2, p3, p4, p5, p6, p7) => action(p1)(p2)(p3)(p4)(p5)(p6)(p7);

    /// <summary>
    /// Transforms a curried action into a action that takes multiple arguments.
    /// </summary>
    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Uncurry<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> action)
        => (p1, p2, p3, p4, p5, p6, p7, p8) => action(p1)(p2)(p3)(p4)(p5)(p6)(p7)(p8);
}
