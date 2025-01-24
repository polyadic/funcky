namespace Funcky;

public static partial class Functional
{
    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <example>
    /// Currying a method group using <see cref="Fn{T}"/> to help with inference:
    /// <code><![CDATA[
    /// var pow = Curry(Fn(Math.Pow));
    /// ]]></code>
    /// </example>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> function)
        => p1 => p2 => function(p1, p2);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <example>
    /// Currying a method group using <see cref="Fn{T}"/> to help with inference:
    /// <code><![CDATA[
    /// double LinearFunction(double m, double c, double x) => (m * x) + c;
    /// var fn = Curry(Fn(LinearFunction));
    /// ]]></code>
    /// </example>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        => p1 => p2 => p3 => function(p1, p2, p3);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
        => p1 => p2 => p3 => p4 => function(p1, p2, p3, p4);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function)
        => p1 => p2 => p3 => p4 => p5 => function(p1, p2, p3, p4, p5);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> Curry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function)
        => p1 => p2 => p3 => p4 => p5 => p6 => function(p1, p2, p3, p4, p5, p6);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        => p1 => p2 => p3 => p4 => p5 => p6 => p7 => function(p1, p2, p3, p4, p5, p6, p7);

    /// <summary>
    /// Curries the given function.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        => p1 => p2 => p3 => p4 => p5 => p6 => p7 => p8 => function(p1, p2, p3, p4, p5, p6, p7, p8);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Action<T2>> Curry<T1, T2>(Action<T1, T2> action)
        => p1 => p2 => action(p1, p2);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Action<T3>>> Curry<T1, T2, T3>(Action<T1, T2, T3> action)
        => p1 => p2 => p3 => action(p1, p2, p3);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        => p1 => p2 => p3 => p4 => action(p1, p2, p3, p4);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> Curry<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
        => p1 => p2 => p3 => p4 => p5 => action(p1, p2, p3, p4, p5);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> Curry<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
        => p1 => p2 => p3 => p4 => p5 => p6 => action(p1, p2, p3, p4, p5, p6);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
        => p1 => p2 => p3 => p4 => p5 => p6 => p7 => action(p1, p2, p3, p4, p5, p6, p7);

    /// <summary>
    /// Curries the given action.
    /// </summary>
    /// <seealso cref="Fn{T}"/>
    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => p1 => p2 => p3 => p4 => p5 => p6 => p7 => p8 => action(p1, p2, p3, p4, p5, p6, p7, p8);
}
