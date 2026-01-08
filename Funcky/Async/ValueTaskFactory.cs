using System.Runtime.CompilerServices;

namespace Funcky.Async;

internal static class ValueTaskFactory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<TResult> ValueTaskFromResult<TResult>(TResult result)
        => new(result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<TResult> ValueTaskFromResult<TResult>(TResult result, CancellationToken cancellationToken)
        => new(result);
}
