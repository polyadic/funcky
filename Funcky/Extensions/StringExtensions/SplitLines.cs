using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Splits a string into individual lines lazily, by any new line (CR, LF, CRLF).
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>A lazy IEnumerable containing the lines.</returns>
        [Pure]
        public static IEnumerable<string> SplitLines(this string text)
            => text.SplitBy(ExtractBy(GetNextLine));

        private static Option<SplitResult> GetNextLine(string text, int startIndex)
            => text
                .IndexOfAnyOrNone(new[] { '\r', '\n' }, startIndex)
                .Match(
                    none: EndOfString(startIndex, text),
                    some: NewLineFound(text, startIndex));

        private static Func<Option<SplitResult>> EndOfString(int startIndex, string text)
            => ()
                => startIndex < text.Length
                ? new SplitResult(text.Length + 1, text.Substring(startIndex))
                : Option<SplitResult>.None();

        private static Func<int, Option<SplitResult>> NewLineFound(string text, int startIndex)
            => index
                => new SplitResult(index + SeparatorLength(text, index), text.Substring(startIndex, index - startIndex));

        private static int SeparatorLength(string text, int index)
            => IsCrLf(text, index)
                ? 2
                : 1;

        private static bool IsCrLf(string text, int index)
            => index + 1 < text.Length
               && text[index] is '\r'
               && text[index + 1] is '\n';
    }
}
