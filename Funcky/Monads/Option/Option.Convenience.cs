namespace Funcky.Monads;

public readonly partial struct Option<TItem>
{
    public static implicit operator Option<TItem>(TItem item)
        => Option.Some(item);

    [Pure]
    public Option<TItem> Where(Func<TItem, bool> predicate)
        => SelectMany(item => Option.FromBoolean(predicate(item), item));

    [Pure]
    public Option<TItem> OrElse(Option<TItem> fallback)
        => Match(none: fallback, some: Option.Some);

    [Pure]
    public Option<TItem> OrElse(Func<Option<TItem>> fallback)
        => Match(none: fallback, some: Option.Some);

    [Pure]
    public TItem GetOrElse(TItem fallback)
        => Match(none: fallback, some: Identity);

    [Pure]
    public TItem GetOrElse(Func<TItem> fallback)
        => Match(none: fallback, some: Identity);

    [Pure]
    public Option<TResult> AndThen<TResult>(Func<TItem, TResult> selector)
        where TResult : notnull
        => Select(selector);

    [Pure]
    public Option<TResult> AndThen<TResult>(Func<TItem, Option<TResult>> selector)
        where TResult : notnull
        => SelectMany(selector);

    /// <summary>
    /// Performs a side effect when the option has a value.
    /// </summary>
    public void AndThen(Action<TItem> action)
        => Switch(none: NoOperation, some: action);

    /// <summary>
    /// Performs a side effect when the option has a value and returns the option again.
    /// This is the <see cref="Option{T}"/> equivalent of <see cref="EnumerableExtensions.Inspect{T}(IEnumerable{T}, Action{T})"/>.
    /// </summary>
    public Option<TItem> Inspect(Action<TItem> inspector)
    {
        AndThen(inspector);
        return this;
    }

    /// <summary>
    /// Performs a side effect when the option has no value (i.e. when the option is <see cref="None"/>) and returns the option again.
    /// </summary>
    public Option<TItem> InspectNone(Action inspector)
    {
        Switch(none: inspector, some: NoOperation);
        return this;
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> that yields exactly one value when the option
    /// has an item and nothing when the option is empty.
    /// </summary>
    [Pure]
    public IEnumerable<TItem> ToEnumerable()
        => Match(
            none: Enumerable.Empty<TItem>,
            some: Sequence.Return);
}
