namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    // All overloads for Fuctions with 2 paramters.
    public static Func<T1, TResult> Apply<T1, T2, TResult>(this Func<T1, T2, TResult> function, Unit arg1, T2 arg2)
        => (a1) => function(a1, arg2);

    public static Func<T2, TResult> Apply<T1, T2, TResult>(this Func<T1, T2, TResult> function, T1 arg1, Unit arg2)
        => (a2) => function(arg1, a2);

    // All overloads for Fuctions with 3 paramters.
    public static Func<T2, T3, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, T1 arg1, Unit arg2, Unit arg3)
        => (a2, a3) => function(arg1, a2, a3);

    public static Func<T1, T3, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, Unit arg1, T2 arg2, Unit arg3)
        => (a1, a3) => function(a1, arg2, a3);

    public static Func<T1, T2, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, Unit arg1, Unit arg2, T3 arg3)
        => (a1, a2) => function(a1, a2, arg3);

    public static Func<T3, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, Unit arg3)
        => a3 => function(arg1, arg2, a3);

    public static Func<T2, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, T1 arg1, Unit arg2, T3 arg3)
        => a2 => function(arg1, a2, arg3);

    public static Func<T1, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function, Unit arg1, T2 arg2, T3 arg3)
        => a1 => function(a1, arg2, arg3);
}
