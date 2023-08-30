using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class ChunkSizeValidator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Validate(int size)
        => size > 0
            ? size
            : throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be bigger than 0");
}
