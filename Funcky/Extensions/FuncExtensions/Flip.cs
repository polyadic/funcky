namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<T1, T2, TResult>(Func<T1, T2, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, T4, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, T4, T5, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, T4, T5, T6, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, T4, T5, T6, T7, TResult> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
    {
        /// <summary>
        /// Flips the first two arguments of the function.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T2, T1, T3, T4, T5, T6, T7, T8, TResult> Flip()
            => Functional.Flip(function);
    }
}
