using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 3: today's struct layout (<c>bool</c> + <c>T</c>), declared as a <em>custom union</em>: <c>[Union]</c> plus
/// <c>IUnion</c> with an <c>object Value</c> that reports the current case. RC3 then treats the struct like a declared
/// union: <c>option is T value</c> / <c>option is None</c> patterns, exhaustiveness (CS8509 for a missing case), and
/// conversions from the case types through the constructors.
/// <para>What it keeps from today: no allocation at rest, in construction, in <c>Match</c>/<c>Select</c>;
/// <c>default</c> is <c>None</c> because <c>Value</c> says so (no third state); <c>sizeof</c> grows with the payload;
/// the list pattern, <c>Option&lt;T&gt;.None</c> as a property, implicit conversion from <c>T</c>, all unchanged.</para>
/// <para>What it costs: a pattern match goes through <c>Value</c>, so a value-type payload is boxed once per match
/// (24 bytes for <c>int</c>); a reference payload and <c>None</c> (cached box) are free. Match/Select do not box.</para>
/// <para>Open: the attribute and interface live in .NET 11's System.Runtime.CompilerServices; whether the compiler
/// accepts an internal polyfill for older frameworks is a separate probe.</para>
/// </summary>
[Union]
public readonly struct CustomUnionOption<T> : IUnion, IEquatable<CustomUnionOption<T>>
    where T : notnull
{
    private readonly bool _hasItem;
    private readonly T _item;

    public CustomUnionOption(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _hasItem = true;
        _item = item;
    }

    public CustomUnionOption(None none)
    {
        _item = default!;
    }

    public static CustomUnionOption<T> None => default;

    /// <summary>The union contract: the current case as an object. The compiler lowers every case pattern to this.</summary>
    public object Value => _hasItem ? _item : NoneBox.Instance;

    public static implicit operator CustomUnionOption<T>(T item) => new(item);

    public static implicit operator CustomUnionOption<T>(None none) => new(none);

    public TResult Match<TResult>(TResult none, Func<T, TResult> some)
        => _hasItem ? some(_item) : none;

    public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        => _hasItem ? some(_item) : none();

    public CustomUnionOption<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => _hasItem ? selector(_item) : CustomUnionOption<TResult>.None;

    public CustomUnionOption<TResult> SelectMany<TResult>(Func<T, CustomUnionOption<TResult>> selector)
        where TResult : notnull
        => _hasItem ? selector(_item) : CustomUnionOption<TResult>.None;

    public T GetOrElse(T fallback) => _hasItem ? _item : fallback;

    public bool TryGetValue([NotNullWhen(true)] out T? item)
    {
        item = _hasItem ? _item : default;
        return _hasItem;
    }

    public bool Equals(CustomUnionOption<T> other)
        => _hasItem == other._hasItem && (!_hasItem || EqualityComparer<T>.Default.Equals(_item, other._item));

    public override bool Equals(object? obj) => obj is CustomUnionOption<T> other && Equals(other);

    public override int GetHashCode() => _hasItem ? _item.GetHashCode() : 0;

    public override string ToString() => _hasItem ? $"Some({_item})" : "None";

    private static class NoneBox
    {
        public static readonly object Instance = new None();
    }
}

public static class CustomUnionOption
{
    public static CustomUnionOption<T> Some<T>(T item)
        where T : notnull
        => new(item);
}
