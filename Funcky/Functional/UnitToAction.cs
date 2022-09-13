namespace Funcky;

public static partial class Functional
{
    [Pure]
    public static Action UnitToAction(Func<Unit> unitFunction)
        => ()
            => unitFunction();

    [Pure]
    public static Action<T1> UnitToAction<T1>(Func<T1, Unit> unitFunction)
        => p1
            => unitFunction(p1);

    [Pure]
    public static Action<T1, T2> UnitToAction<T1, T2>(Func<T1, T2, Unit> unitFunction)
        => (p1, p2)
            => unitFunction(p1, p2);

    [Pure]
    public static Action<T1, T2, T3> UnitToAction<T1, T2, T3>(Func<T1, T2, T3, Unit> unitFunction)
        => (p1, p2, p3)
            => unitFunction(p1, p2, p3);

    [Pure]
    public static Action<T1, T2, T3, T4> UnitToAction<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Unit> unitFunction)
        => (p1, p2, p3, p4)
            => unitFunction(p1, p2, p3, p4);

    [Pure]
    public static Action<T1, T2, T3, T4, T5> UnitToAction<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Unit> unitFunction)
        => (p1, p2, p3, p4, p5)
            => unitFunction(p1, p2, p3, p4, p5);

    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6> UnitToAction<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Unit> unitFunction)
        => (p1, p2, p3, p4, p5, p6)
            => unitFunction(p1, p2, p3, p4, p5, p6);

    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7> UnitToAction<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, Unit> unitFunction)
        => (p1, p2, p3, p4, p5, p6, p7)
            => unitFunction(p1, p2, p3, p4, p5, p6, p7);

    [Pure]
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> UnitToAction<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> unitFunction)
        => (p1, p2, p3, p4, p5, p6, p7, p8)
            => unitFunction(p1, p2, p3, p4, p5, p6, p7, p8);
}
