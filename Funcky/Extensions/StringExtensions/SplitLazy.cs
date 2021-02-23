using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        private delegate Option<SplitResult> ExtractElement(string text, int startIndex);

        private delegate Option<int> FindNextIndex(string text, int startIndex);

        private delegate int GetLength(int index, bool hasCarriageReturn);

        /// <summary>
        /// Splits a string into individual parts by a given separator.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="separator">A single character separating the parts.</param>
        /// <returns>A lazy IEnumerable containing the parts.</returns>
        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, char separator)
            => text.SplitBy(ExtractByIndex(IndexOfCharSeparator(separator)));

        /// <summary>
        /// Splits a string into individual parts by several given separators.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="separators">Different characters separating the parts.</param>
        /// <returns>A lazy IEnumerable containing the parts.</returns>
        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, params char[] separators)
            => text.SplitBy(ExtractByIndex(IndexOfCharSeparators(separators)));

        /// <summary>
        /// Splits a string into individual parts by a given separator.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="separator">A single string separating the parts.</param>
        /// <returns>A lazy IEnumerable containing the parts.</returns>
        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, string separator)
            => text.SplitBy(ExtractByIndex(IndexOfStringSeparator(separator)));

        /// <summary>
        /// Splits a string into individual parts by several given separators.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="separators">Different strings separating the parts.</param>
        /// <returns>A lazy IEnumerable containing the parts.</returns>
        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, params string[] separators)
            => text.SplitBy(ExtractByIndex(IndexOfStringSeparators(separators)));

        private static IEnumerable<string> SplitBy(this string text, ExtractElement extractNext)
        {
            if (text == string.Empty)
            {
                yield break;
            }

            for (var startIndex = 0; extractNext(text, startIndex).TryGetValue(out var splitResult);)
            {
                yield return splitResult.Result;

                startIndex = splitResult.NextStartIndex;
            }
        }

        private static FindNextIndex IndexOfCharSeparator(char separator)
            => (text, startIndex)
                => text.IndexOfOrNone(separator, startIndex);

        private static FindNextIndex IndexOfCharSeparators(char[] separator)
            => (text, startIndex)
                => text.IndexOfAnyOrNone(separator, startIndex);

        private static FindNextIndex IndexOfStringSeparator(string separator)
            => (text, startIndex)
                => text.IndexOfOrNone(separator, startIndex);

        private static FindNextIndex IndexOfStringSeparators(string[] separators)
            => (text, startIndex)
                => separators.AsEnumerable().Select(s => text.IndexOfOrNone(s)).MinOrNone();

        private static ExtractElement ExtractByIndex(FindNextIndex findNextIndex)
            => ExtractBy(GetIndex(findNextIndex));

        private static ExtractElement ExtractBy(ExtractElement extractElement)
            => (text, startIndex)
                => startIndex <= text.Length
                    ? extractElement(text, startIndex)
                    : Option<SplitResult>.None();

        private static ExtractElement GetIndex(FindNextIndex getIndex)
            => (text, startIndex)
                => getIndex(text, startIndex)
                    .Match(
                        none: () => new SplitResult(text.Substring(startIndex), text.Length + 1),
                        some: index => new SplitResult(text.Substring(startIndex, index - startIndex), index + 1));

        private sealed class SplitResult
        {
            public SplitResult(string result, int nextStartIndex)
            {
                Result = result;
                NextStartIndex = nextStartIndex;
            }

            public string Result { get; }

            public int NextStartIndex { get; }
        }
    }
}
