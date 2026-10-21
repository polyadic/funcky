#if !NET6_0_OR_GREATER
namespace Funcky.Xunit.Test;

internal static class StringExtensions
{
    public static string ReplaceLineEndings(this string input, string replacementText)
        => input.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", replacementText);
}
#endif
