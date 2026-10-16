using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class RepeatCountValidator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Validate(int count)
        => count >= 0
            ? count
            : throw new ArgumentOutOfRangeException(nameof(count), count, "Count must not be negative");
}
