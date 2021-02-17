using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        private delegate Option<SplitResult> ExtractElement(string text, int startIndex);

        [Pure]
        public static IEnumerable<string> SplitLines(this string text)
            => text.SplitBy(ExtractLine);

        [Pure]
        public static IEnumerable<string> SplitLazy(this string text, string[] seperators)
            => text.SplitBy(Extract(seperators));

        private static ExtractElement Extract(string[] seperators)
            => (text, startIndex)
                => new SplitResult(0, text.Length, text.Length);

        [Pure]
        private static IEnumerable<string> SplitBy(this string text, ExtractElement extractNext)
        {
            var startIndex = 0;
            while (extractNext(text, startIndex).TryGetValue(out var splitResult))
            {
                yield return text.Substring(splitResult.Begin, splitResult.Length);

                startIndex = splitResult.NextStartIndex;
            }
        }

        private static Option<SplitResult> ExtractLine(string text, int startIndex)
            => startIndex < text.Length
                ? new Option<SplitResult>(new SplitResult(startIndex, 1, startIndex + 1))
                : Option<SplitResult>.None();

        private sealed class SplitResult
        {
            public SplitResult(int begin, int length, int nextStartIndex)
            {
                Begin = begin;
                Length = length;
                NextStartIndex = nextStartIndex;
            }

            public int Begin { get; }

            public int Length { get; }

            public int NextStartIndex { get; }
        }
    }
}
