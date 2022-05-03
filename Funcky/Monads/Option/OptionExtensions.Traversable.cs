namespace Funcky.Monads;

public static partial class OptionExtensions
{
    [Pure]
    public static Either<TLeft, Option<TRight>> Traverse<TItem, TLeft, TRight>(
        this Option<TItem> option,
        Func<TItem, Either<TLeft, TRight>> selector)
        where TItem : notnull
        where TRight : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Either<TLeft, Option<TItem>> Sequence<TLeft, TItem>(
        this Option<Either<TLeft, TItem>> option)
        where TItem : notnull
        => option.Match(
            none: Either<TLeft>.Return(Option<TItem>.None),
            some: right => right.Select(Option.Return));

    [Pure]
    public static Result<Option<TValidResult>> Traverse<TItem, TValidResult>(
        this Option<TItem> option,
        Func<TItem, Result<TValidResult>> selector)
        where TItem : notnull
        where TValidResult : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Result<Option<TItem>> Sequence<TItem>(
        this Option<Result<TItem>> option)
        where TItem : notnull
        => option.Match(
            none: Result.Return(Option<TItem>.None),
            some: item => item.Select(Option.Return));

    [Pure]
    public static Lazy<Option<T>> Traverse<TItem, T>(
        this Option<TItem> option,
        Func<TItem, Lazy<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Lazy<Option<TItem>> Sequence<TItem>(
        this Option<Lazy<TItem>> option)
        where TItem : notnull
        => option.Match(
            none: Lazy.Return(Option<TItem>.None),
            some: item => item.Select(Option.Return));

    [Pure]
    public static IEnumerable<Option<T>> Traverse<TItem, T>(
        this Option<TItem> option,
        Func<TItem, IEnumerable<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static IEnumerable<Option<TItem>> Sequence<TItem>(
        this Option<IEnumerable<TItem>> option)
        where TItem : notnull
        => option.Match(
            none: Funcky.Sequence.Return(Option<TItem>.None),
            some: item => item.Select(Option.Return));

    [Pure]
    public static Reader<TEnvironment, Option<TResult>> Traverse<TItem, TEnvironment, TResult>(
        this Option<TItem> option,
        Func<TItem, Reader<TEnvironment, TResult>> selector)
        where TItem : notnull
        where TResult : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Reader<TEnvironment, Option<TItem>> Sequence<TEnvironment, TItem>(
        this Option<Reader<TEnvironment, TItem>> option)
        where TItem : notnull
        => option.Match(
            none: Reader<TEnvironment>.Return(Option<TItem>.None),
            some: item => item.Select(Option.Return));
}
