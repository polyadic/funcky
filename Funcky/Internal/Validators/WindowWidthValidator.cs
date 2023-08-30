using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class WindowWidthValidator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Validate(int width)
        => width > 0
            ? width
            : throw new ArgumentOutOfRangeException(nameof(width), width, "The width of the window must be larger than 0");
}
