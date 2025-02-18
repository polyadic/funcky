using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ListExtensions
{
    [Pure]
    public static Option<int> IndexOfOrNone<TValue>(this IList<TValue> list, TValue value)
        => MapNotFoundToNone(list.IndexOf(value));
}
