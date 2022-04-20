namespace Funcky.Monads;

public readonly partial struct Option<TItem>
{
    [Pure]
    public Option<TResult> Select<TResult>(Func<TItem, TResult> selector)
        where TResult : notnull
        => Match(
             none: Option.None<TResult>,
             some: item => selector(item));

    [Pure]
    public Option<TResult> SelectMany<TResult>(Func<TItem, Option<TResult>> selector)
        where TResult : notnull
        => Match(
             none: Option.None<TResult>,
             some: selector);

    [Pure]
    public Option<TResult> SelectMany<TMaybe, TResult>(Func<TItem, Option<TMaybe>> selector, Func<TItem, TMaybe, TResult> resultSelector)
        where TResult : notnull
        where TMaybe : notnull
        => SelectMany(
             item => selector(item).Select(
                 maybe => resultSelector(item, maybe)));
}
