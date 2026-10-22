using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ListExtensions
{
    extension<TValue>(IList<TValue> list)
    {
        [Pure]
        public Option<int> IndexOfOrNone(TValue value)
            => MapNotFoundToNone(list.IndexOf(value));
    }
}
