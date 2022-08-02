using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

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
            some: static right => right.Select(Option.Return));

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
            none: static () => Result.Return(Option<TItem>.None),
            some: static item => item.Select(Option.Return));

    [Pure]
    public static Lazy<Option<T>> Traverse<TItem, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(
        this Option<TItem> option,
        Func<TItem, Lazy<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Lazy<Option<TItem>> Sequence<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TItem>(
        this Option<Lazy<TItem>> option)
        where TItem : notnull
         => option.Match(
             none: Lazy.Return(Option<TItem>.None),
             some: SequenceLazy<TItem>.Some);

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
            none: static () => Funcky.Sequence.Return(Option<TItem>.None),
            some: static item => item.Select(Option.Return));

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
            none: static () => Reader<TEnvironment>.Return(Option<TItem>.None),
            some: static item => item.Select(Option.Return));

    // Workaround for https://github.com/dotnet/linker/issues/1416
    private static class SequenceLazy<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TItem>
        where TItem : notnull
    {
        private static Func<Lazy<TItem>, Lazy<Option<TItem>>>? _some;

        public static Func<Lazy<TItem>, Lazy<Option<TItem>>> Some => _some ??= (some => some.Select(Option.Return));
    }
}
