using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Analyzers;

/// <summary>A very minimal option implementation
/// intended for use with pattern matching.</summary>
[CollectionBuilder(typeof(Option), nameof(Option.Create))]
internal readonly partial struct Option<TItem> : IEnumerable<TItem>
    where TItem : notnull
{
    private readonly bool _hasItem;
    private readonly TItem _item;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Option(TItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _item = item;
        _hasItem = true;
    }

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
        if (_hasItem)
        {
            yield return _item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<TItem>).GetEnumerator();
}

internal static class Option
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<TItem> Create<TItem>(ReadOnlySpan<TItem> values)
        where TItem : notnull
        => values.Length switch
        {
            0 => default,
            1 => new Option<TItem>(values[0]),
            _ => throw new ArgumentException("An option can contain only zero or one elements"),
        };
}
