using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Analyzers;

/// <summary>A very minimal option implementation
/// intended for use with pattern matching.</summary>
[CollectionBuilder(typeof(Option), nameof(Option.Create))]
internal readonly partial struct Option<TItem>
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

    // Enumerator is required by collection expression.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Enumerator GetEnumerator() => new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Enumerator
    {
        private readonly TItem _item;
        private bool _hasItem;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Enumerator(Option<TItem> option)
        {
            _item = option._item;
            _hasItem = option._hasItem;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly TItem Current => _item;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MoveNext()
        {
            var hasItem = _hasItem;
            _hasItem = false;
            return hasItem;
        }
    }
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
