using System;
using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> function) =>
            (T1 p1) => (T2 p2) => function(p1, p2);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => function(p1, p2, p3);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => (T4 p4) => function(p1, p2, p3, p4);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => (T4 p4) => (T5 p5) => function(p1, p2, p3, p4, p5);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> Curry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => (T4 p4) => (T5 p5) => (T6 p6) => function(p1, p2, p3, p4, p5, p6);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => (T4 p4) => (T5 p5) => (T6 p6) => (T7 p7) => function(p1, p2, p3, p4, p5, p6, p7);

        /// <summary>
        /// Curries the given the function.
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function) =>
            (T1 p1) => (T2 p2) => (T3 p3) => (T4 p4) => (T5 p5) => (T6 p6) => (T7 p7) => (T8 p8) => function(p1, p2, p3, p4, p5, p6, p7, p8);
    }
}
