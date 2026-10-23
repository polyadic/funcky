namespace Funcky.Extensions;

public static partial class ActionExtensions
{
    extension<T1, T2>(Func<T1, Action<T2>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3>(Func<T1, Func<T2, Action<T3>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3, T4>(Func<T1, Func<T2, Func<T3, Action<T4>>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3, T4> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3, T4, T5>(Func<T1, Func<T2, Func<T3, Func<T4, Action<T5>>>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3, T4, T5> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3, T4, T5, T6>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Action<T6>>>>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3, T4, T5, T6> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Action<T7>>>>>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3, T4, T5, T6, T7> Uncurry()
            => Functional.Uncurry(action);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Action<T8>>>>>>>> action)
    {
        /// <summary>
        /// Transforms a curried action into an action that takes multiple arguments.
        /// </summary>
        /// <seealso cref="Fn{T}"/>
        [Pure]
        public Action<T1, T2, T3, T4, T5, T6, T7, T8> Uncurry()
            => Functional.Uncurry(action);
    }
}
