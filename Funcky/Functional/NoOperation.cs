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
    public static void NoOperation<T1>(T1 ω1)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2>(T1 ω1, T2 ω2)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3>(T1 ω1, T2 ω2, T3 ω3)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4>(T1 ω1, T2 ω2, T3 ω3, T4 ω4)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6, T7>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6, T7 ω7)
        => NoOperation();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoOperation<T1, T2, T3, T4, T5, T6, T7, T8>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6, T7 ω7, T8 ω8)
        => NoOperation();
}
