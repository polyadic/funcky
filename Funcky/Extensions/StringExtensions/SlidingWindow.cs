using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    /// <summary>
    /// SlidingWindow returns a sequence of sliding window strings of the given width.
    /// The nth string will start with the nth character of the source sequence.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="width">The width of the sliding window in characters.</param>
    /// <returns>Returns a sequence of equally sized substrings.</returns>
    public static IEnumerable<string> SlidingWindow(this string source, int width)
        => StringSlidingWindow(source, WindowWidthValidator.Validate(width));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<string> StringSlidingWindow(string source, int width)
    {
        var numberOfWindows = source.Length - width + 1;
        for (var index = 0; index < numberOfWindows; ++index)
        {
            yield return source.Substring(index, width);
        }
    }
}
