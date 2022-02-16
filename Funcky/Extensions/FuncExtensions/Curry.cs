namespace Funcky.Extensions
{
    public static partial class FuncExtensions
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> function)
            => p1 => p2 => function(p1, p2);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function)
            => p1 => p2 => p3 => function(p1, p2, p3);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function)
            => p1 => p2 => p3 => p4 => function(p1, p2, p3, p4);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> function)
            => p1 => p2 => p3 => p4 => p5 => function(p1, p2, p3, p4, p5);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> Curry<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> function)
            => p1 => p2 => p3 => p4 => p5 => p6 => function(p1, p2, p3, p4, p5, p6);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
            => p1 => p2 => p3 => p4 => p5 => p6 => p7 => function(p1, p2, p3, p4, p5, p6, p7);

        /// <summary>
        /// Curries the given function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
            => p1 => p2 => p3 => p4 => p5 => p6 => p7 => p8 => function(p1, p2, p3, p4, p5, p6, p7, p8);
    }
}
