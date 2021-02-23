using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        private delegate int GetLength(int index, bool hasCarriageReturn);

        /// <summary>
        /// Splits a string into individual lines lazily, by any new line (CR, LF, CRLF).
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>A lazy IEnumerable containing the lines.</returns>
        [Pure]
        public static IEnumerable<string> SplitLines(this string text)
            => text.SplitBy(ExtractBy(GetNextLine));

        private static Option<SplitResult> GetNextLine(string text, int startIndex)
            => GetNextLine(GetLengthFrom(startIndex))(text, startIndex);

        private static ExtractElement GetNextLine(GetLength getLength)
            => (text, startIndex)
                =>
                {
                    var seenCarriageReturn = false;
                    for (var index = startIndex; ; ++index)
                    {
                        if (IsEndOfLine(text, index) || seenCarriageReturn)
                        {
                            return new SplitResult(NextStartIndex(index, IsEndOfLine(text, index)), text.Substring(startIndex, getLength(index, seenCarriageReturn)));
                        }

                        seenCarriageReturn = text[index] is '\r';
                    }
                };

        private static GetLength GetLengthFrom(int startIndex)
            => (index, hasCarriageReturn)
                => hasCarriageReturn
                    ? index - startIndex - 1
                    : index - startIndex;

        private static bool IsEndOfLine(string text, int index)
            => index == text.Length
               || text[index] is '\n';

        private static int NextStartIndex(int index, bool indexIsAtTheEndOfALine)
            => indexIsAtTheEndOfALine
                ? index + 1
                : index;
    }
}
