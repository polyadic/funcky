using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    extension(string haystack)
    {
        [Pure]
        public Option<int> LastIndexOfOrNone(char value)
            => MapNotFoundToNone(haystack.LastIndexOf(value));

        [Pure]
        public Option<int> LastIndexOfOrNone(char value, int startIndex)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex));

        [Pure]
        public Option<int> LastIndexOfOrNone(char value, int startIndex, int count)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value)
            => MapNotFoundToNone(haystack.LastIndexOf(value));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value, int startIndex)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value, int startIndex, int count)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.LastIndexOf(value, comparisonType));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value, int startIndex, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, comparisonType));

        [Pure]
        public Option<int> LastIndexOfOrNone(string value, int startIndex, int count, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.LastIndexOf(value, startIndex, count, comparisonType));
    }
}
