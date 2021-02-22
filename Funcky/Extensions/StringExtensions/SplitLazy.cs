using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        private delegate Option<SplitResult> ExtractElement(string text, int startIndex);

        [Pure]
        public static IEnumerable<string> SplitLines(this string text, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
            => text.SplitBy(ExtractLine);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, string[] separators)
            => text.SplitBy(Extract(separators));

        [Pure]
        private static IEnumerable<string> SplitBy(this string text, ExtractElement extractNext)
        {
            if (text == string.Empty)
            {
                yield break;
            }

            var startIndex = 0;
            while (extractNext(text, startIndex).TryGetValue(out var splitResult))
            {
                yield return splitResult.Result;

                startIndex = splitResult.NextStartIndex;
            }
        }

        private static ExtractElement Extract(string[] separators)
            => (text, startIndex)
                 => new SplitResult("todo", startIndex + 1);

        private static Option<SplitResult> ExtractLine(string text, int startIndex)
            => startIndex <= text.Length
                ? GetNextLine(text, startIndex)
                : Option<SplitResult>.None();

        private static Option<SplitResult> GetNextLine(string text, int startIndex)
        {
            var getLength = GetLengthFrom(startIndex);
            var seenCarriageReturn = false;
            for (var index = startIndex; ; ++index)
            {
                if (IsEndOfLine(text, index) || seenCarriageReturn)
                {
                    return new SplitResult(text.Substring(startIndex, getLength(index, seenCarriageReturn)), NextStartIndex(index, IsEndOfLine(text, index)));
                }

                seenCarriageReturn = text[index] is '\r';
            }
        }

        private static bool IsEndOfLine(string text, int index)
            => index == text.Length
                || text[index] is '\n';

        private static int NextStartIndex(int index, bool indexIsAtTheEndOfALine)
            => indexIsAtTheEndOfALine
                ? index + 1
                : index;

        private static Func<int, bool, int> GetLengthFrom(int startIndex)
            => (index, hasCarriageReturn)
                => hasCarriageReturn
                    ? index - startIndex - 1
                    : index - startIndex;

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
