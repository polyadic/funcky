using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    private const int LastElement = 1;

    /// <summary>
    /// Chunks the source string into equally sized chunks. The last chunk can be smaller.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="size">The desired size of the chunks in characters.</param>
    /// <returns>A sequence of equally sized substrings containing consecutive substrings of the source string in the same order.</returns>
    public static IEnumerable<string> Chunk(this string source, int size)
        => ChunkString(source, ChunkSizeValidator.Validate(size));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<string> ChunkString(string source, int size)
    {
        var count = DivisionWithCeiling(source.Length, size);

        // optimization: we do not emit the last chunk, because it might be smaller than size.
        var index = 0;
        for (; index < count - LastElement; ++index)
        {
            yield return source.Substring(index * size, size);
        }

        // If there is anything left to emit, we will emit the last chunk.
        if (source is not "")
        {
            yield return source.Substring(index * size);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DivisionWithCeiling(int dividend, int divisor)
        => (dividend + divisor - 1) / divisor;
}
