namespace Funcky.Monads;

public readonly partial struct Option<TItem>
{
    [Pure]
    public Option<TResult> Select<TResult>(Func<TItem, TResult> selector)
        where TResult : notnull
        => Match(
             none: Option<TResult>.None,
             some: item => selector(item));

    [Pure]
    public Option<TResult> SelectMany<TResult>(Func<TItem, Option<TResult>> selector)
        where TResult : notnull
        => Match(
             none: Option<TResult>.None,
             some: selector);

    [Pure]
    public Option<TResult> SelectMany<TOption, TResult>(Func<TItem, Option<TOption>> selector, Func<TItem, TOption, TResult> resultSelector)
        where TResult : notnull
        where TOption : notnull
        => SelectMany(
             item => selector(item).Select(
                 option => resultSelector(item, option)));
}
