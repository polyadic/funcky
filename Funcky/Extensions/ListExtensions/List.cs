using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ListExtensions
{
    extension<TValue>(List<TValue> list)
    {
        [Pure]
        public Option<int> FindIndexOrNone(Predicate<TValue> match)
            => MapNotFoundToNone(list.FindIndex(match));

        [Pure]
        public Option<int> FindIndexOrNone(int startIndex, Predicate<TValue> match)
            => MapNotFoundToNone(list.FindIndex(startIndex, match));

        [Pure]
        public Option<int> FindIndexOrNone(int startIndex, int count, Predicate<TValue> match)
            => MapNotFoundToNone(list.FindIndex(startIndex, count, match));

        [Pure]
        public Option<int> FindLastIndexOrNone(Predicate<TValue> match)
            => MapNotFoundToNone(list.FindLastIndex(match));

        [Pure]
        public Option<int> FindLastIndexOrNone(int startIndex, Predicate<TValue> match)
            => MapNotFoundToNone(list.FindLastIndex(startIndex, match));

        [Pure]
        public Option<int> FindLastIndexOrNone(int startIndex, int count, Predicate<TValue> match)
            => MapNotFoundToNone(list.FindLastIndex(startIndex, count, match));
    }
}
