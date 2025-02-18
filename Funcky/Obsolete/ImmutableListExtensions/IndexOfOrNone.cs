using System.Collections.Immutable;
using System.ComponentModel;
using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete($"Use {nameof(ListExtensions)} instead.")]
public static partial class ImmutableListExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(IndexOfOrNone)} instead.")]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.IndexOf(item, startIndex, count, equalityComparer));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(IndexOfOrNone)} instead.")]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item)
        => MapNotFoundToNone(list.IndexOf(item));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(IndexOfOrNone)} instead.")]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.IndexOf(item, equalityComparer));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(IndexOfOrNone)} instead.")]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex)
        => MapNotFoundToNone(list.IndexOf(item, startIndex));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(IndexOfOrNone)} instead.")]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count)
        => MapNotFoundToNone(list.IndexOf(item, startIndex, count));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(LastIndexOfOrNone)} instead.")]
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex, count, equalityComparer));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(LastIndexOfOrNone)} instead.")]
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item)
        => MapNotFoundToNone(list.LastIndexOf(item));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(LastIndexOfOrNone)} instead.")]
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.LastIndexOf(item, equalityComparer));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(LastIndexOfOrNone)} instead.")]
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex));

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Use {nameof(ListExtensions)}.{nameof(LastIndexOfOrNone)} instead.")]
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex, count));
}
