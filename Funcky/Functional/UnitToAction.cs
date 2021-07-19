using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        [Pure]
        public static Action UnitToAction(Func<Unit> action)
            => ()
                => action();

        [Pure]
        public static Action<T1> UnitToAction<T1>(Func<T1, Unit> action)
            => p1
                => action(p1);

        [Pure]
        public static Action<T1, T2> UnitToAction<T1, T2>(Func<T1, T2, Unit> action)
            => (p1, p2)
                => action(p1, p2);

        [Pure]
        public static Action<T1, T2, T3> UnitToAction<T1, T2, T3>(Func<T1, T2, T3, Unit> action)
            => (p1, p2, p3)
                => action(p1, p2, p3);

        [Pure]
        public static Action<T1, T2, T3, T4> UnitToAction<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Unit> action)
            => (p1, p2, p3, p4)
                => action(p1, p2, p3, p4);

        [Pure]
        public static Action<T1, T2, T3, T4, T5> UnitToAction<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Unit> action)
            => (p1, p2, p3, p4, p5)
                => action(p1, p2, p3, p4, p5);

        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6> UnitToAction<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Unit> action)
            => (p1, p2, p3, p4, p5, p6)
                => action(p1, p2, p3, p4, p5, p6);

        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6, T7> UnitToAction<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, Unit> action)
            => (p1, p2, p3, p4, p5, p6, p7)
                => action(p1, p2, p3, p4, p5, p6, p7);

        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> UnitToAction<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> action)
            => (p1, p2, p3, p4, p5, p6, p7, p8)
                => action(p1, p2, p3, p4, p5, p6, p7, p8);
    }
}
