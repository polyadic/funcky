using System.Runtime.CompilerServices;

namespace Funcky.Functional;

public static partial class Functional
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync()
        => Task.CompletedTask;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1>(T1 ω1)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2>(T1 ω1, T2 ω2)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3>(T1 ω1, T2 ω2, T3 ω3)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3, T4>(T1 ω1, T2 ω2, T3 ω3, T4 ω4)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3, T4, T5>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6, T7>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6, T7 ω7)
        => NoOperationAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6, T7, T8>(T1 ω1, T2 ω2, T3 ω3, T4 ω4, T5 ω5, T6 ω6, T7 ω7, T8 ω8)
        => NoOperationAsync();
}
