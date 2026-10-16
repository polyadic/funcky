using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class MaxRetriesValidator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Validate(int maxRetries)
        => maxRetries >= 0
            ? maxRetries
            : throw new ArgumentOutOfRangeException(nameof(maxRetries), maxRetries, "The maximum number of retries must not be negative");
}
