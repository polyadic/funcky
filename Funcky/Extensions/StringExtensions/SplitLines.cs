namespace Funcky.Extensions;

public static partial class StringExtensions
{
    private const char CarriageReturn = '\r';
    private const char LineFeed = '\n';

    /// <summary>
    /// Splits a string into individual lines lazily, by any new line (CR, LF, CRLF).
    /// Unlike <c>string.Split</c>, a new line at the very end of the text does not produce a trailing empty line, and an empty text
    /// yields no lines at all. Consecutive new lines produce empty lines. Other Unicode line separators are not recognized.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>A lazy IEnumerable containing the lines.</returns>
    [Pure]
    public static IEnumerable<string> SplitLines(this string text)
        => text.SplitBy(ExtractBy(GetNextLine));

    private static Option<SplitResult> GetNextLine(string text, int startIndex)
        => text
            .IndexOfAnyOrNone([CarriageReturn, LineFeed], startIndex)
            .Match(
                none: EndOfString(startIndex, text),
                some: NewLine(text, startIndex));

    private static Func<Option<SplitResult>> EndOfString(int startIndex, string text)
        => ()
            => Option
                .FromBoolean(startIndex < text.Length)
                .Select(_ => new SplitResult(text.Length + 1, text.Substring(startIndex)));

    private static Func<int, Option<SplitResult>> NewLine(string text, int startIndex)
        => index
            => new SplitResult(index + SeparatorLength(text, index), text.Substring(startIndex, index - startIndex));

    private static int SeparatorLength(string text, int index)
        => IsCrLf(text, index)
            ? 2
            : 1;

    private static bool IsCrLf(string text, int index)
        => index + 1 < text.Length
           && text[index] is CarriageReturn
           && text[index + 1] is LineFeed;
}
