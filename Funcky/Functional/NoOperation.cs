using System.Runtime.CompilerServices;

namespace Funcky;

public static partial class Functional
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation()
    {
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1>(T1 p1)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2>(T1 p1, T2 p2)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        => NoOperation();
}
