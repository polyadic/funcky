namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    extension<T1, T2>(Action<T1, T2> function)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1> Flip()
            => Functional.Flip(function);
    }

    extension<T1, T2, T3>(Action<T1, T2, T3> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3> Flip()
            => Functional.Flip(action);
    }

    extension<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3, T4> Flip()
            => Functional.Flip(action);
    }

    extension<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3, T4, T5> Flip()
            => Functional.Flip(action);
    }

    extension<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3, T4, T5, T6> Flip()
            => Functional.Flip(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3, T4, T5, T6, T7> Flip()
            => Functional.Flip(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
    {
        /// <summary>
        /// Flips the first two arguments of the action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T2, T1, T3, T4, T5, T6, T7, T8> Flip()
            => Functional.Flip(action);
    }
}
