using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class StringExtensions
{
    extension(string haystack)
    {
        [Pure]
        public Option<int> IndexOfOrNone(char value)
            => MapNotFoundToNone(haystack.IndexOf(value));

        [Pure]
        public Option<int> IndexOfOrNone(char value, int startIndex)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex));

        [Pure]
        public Option<int> IndexOfOrNone(char value, int startIndex, int count)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count));

        [Pure]
        public Option<int> IndexOfOrNone(char value, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.IndexOf(value, comparisonType));

        [Pure]
        public Option<int> IndexOfOrNone(string value)
            => MapNotFoundToNone(haystack.IndexOf(value));

        [Pure]
        public Option<int> IndexOfOrNone(string value, int startIndex)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex));

        [Pure]
        public Option<int> IndexOfOrNone(string value, int startIndex, int count)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count));

        [Pure]
        public Option<int> IndexOfOrNone(string value, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.IndexOf(value, comparisonType));

        [Pure]
        public Option<int> IndexOfOrNone(string value, int startIndex, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex, comparisonType));

        [Pure]
        public Option<int> IndexOfOrNone(string value, int startIndex, int count, StringComparison comparisonType)
            => MapNotFoundToNone(haystack.IndexOf(value, startIndex, count, comparisonType));
    }
}
