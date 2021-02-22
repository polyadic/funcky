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
            => text.SplitBy(ExtractLine, stringSplitOptions);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, char separator, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
            => text.SplitBy(Extract(separator), stringSplitOptions);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, char[] separators, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
            => text.SplitBy(Extract(separators), stringSplitOptions);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, string separator, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
            => text.SplitBy(Extract(separator), stringSplitOptions);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, string[] separators, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
            => text.SplitBy(Extract(separators), stringSplitOptions);

        [Pure]
        private static IEnumerable<string> SplitBy(this string text, ExtractElement extractNext, StringSplitOptions stringSplitOptions)
        {
            if (text == string.Empty)
            {
                yield break;
            }

            for (var startIndex = 0; extractNext(text, startIndex).TryGetValue(out var splitResult);)
            {
                var result = TrimWhenNecessary(stringSplitOptions, splitResult);

                if (CanYield(stringSplitOptions, result))
                {
                    yield return result;
                }

                startIndex = splitResult.NextStartIndex;
            }
        }

        private static bool CanYield(StringSplitOptions stringSplitOptions, string result)
        {
            return !stringSplitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries) || result.Length != 0;
        }

#if TRIM_ENTRIES
        private static string TrimWhenNecessary(StringSplitOptions stringSplitOptions, SplitResult splitResult)
        {
            return stringSplitOptions.HasFlag(StringSplitOptions.TrimEntries)
? splitResult.Result.Trim()
: splitResult.Result;
        }
#else
        private static string TrimWhenNecessary(StringSplitOptions stringSplitOptions, SplitResult splitResult)
            => splitResult.Result;
#endif

        private static ExtractElement Extract<T>(T t)
        {
            return (text, startIndex)
                             => new SplitResult("todo", startIndex + 1);
        }

        private static Option<SplitResult> ExtractLine(string text, int startIndex)
        {
            return startIndex <= text.Length
                           ? GetNextLine(text, startIndex)
                           : Option<SplitResult>.None();
        }

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
        {
            return index == text.Length
                           || text[index] is '\n';
        }

        private static int NextStartIndex(int index, bool indexIsAtTheEndOfALine)
        {
            return indexIsAtTheEndOfALine
                           ? index + 1
                           : index;
        }

        private static Func<int, bool, int> GetLengthFrom(int startIndex)
        {
            return (index, hasCarriageReturn)
                            => hasCarriageReturn
                                ? index - startIndex - 1
                                : index - startIndex;
        }

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
