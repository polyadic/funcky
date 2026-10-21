namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    extension<T1, T2>(Action<T1, T2> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Action<T2>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3>(Action<T1, T2, T3> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Action<T3>>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> Curry()
            => Functional.Curry(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
    {
        /// <summary>
        /// Curries the given action.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> Curry()
            => Functional.Curry(action);
    }
}
