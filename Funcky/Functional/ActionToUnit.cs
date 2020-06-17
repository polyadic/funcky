using System;
using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        [Pure]
        public static Func<Unit> ActionToUnit(Action action)
            => () =>
            {
                action();
                return default;
            };

        [Pure]
        public static Func<T, Unit> ActionToUnit<T>(Action<T> action)
        {
            return parameter =>
            {
                action(parameter);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, Unit> ActionToUnit<T1, T2>(Action<T1, T2> action)
        {
            return (p1, p2) =>
            {
                action(p1, p2);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, Unit> ActionToUnit<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return (p1, p2, p3) =>
            {
                action(p1, p2, p3);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, T4, Unit> ActionToUnit<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            return (p1, p2, p3, p4) =>
            {
                action(p1, p2, p3, p4);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, T4, T5, Unit> ActionToUnit<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
        {
            return (p1, p2, p3, p4, p5) =>
            {
                action(p1, p2, p3, p4, p5);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, Unit> ActionToUnit<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
        {
            return (p1, p2, p3, p4, p5, p6) =>
            {
                action(p1, p2, p3, p4, p5, p6);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> ActionToUnit<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            return (p1, p2, p3, p4, p5, p6, p7) =>
            {
                action(p1, p2, p3, p4, p5, p6, p7);
                return default;
            };
        }

        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> ActionToUnit<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            return (p1, p2, p3, p4, p5, p6, p7, p8) =>
            {
                action(p1, p2, p3, p4, p5, p6, p7, p8);
                return default;
            };
        }
    }
}
