namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<T1, T2, TResult>(Func<T1, T2, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, TResult>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, TResult>>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> Curry()
            => Functional.Curry(function);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
    {
        /// <summary>
        /// Curries the given function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> Curry()
            => Functional.Curry(function);
    }
}
