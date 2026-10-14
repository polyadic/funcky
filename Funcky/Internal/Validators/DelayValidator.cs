using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class DelayValidator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Validate(TimeSpan delay, string parameterName)
        => delay >= TimeSpan.Zero
            ? delay
            : throw new ArgumentOutOfRangeException(parameterName, delay, "The delay must not be negative");
}
