using System.Diagnostics.CodeAnalysis;
using static Funcky.Async.ValueTaskFactory;

namespace Funcky.Monads;

public static class OptionAsyncExtensions
{
    [Pure]
    public static IAsyncEnumerable<Option<T>> Traverse<TItem, T>(
        this Option<TItem> option,
        Func<TItem, IAsyncEnumerable<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static IAsyncEnumerable<Option<TItem>> Sequence<TItem>(
        this Option<IAsyncEnumerable<TItem>> option)
        where TItem : notnull
        => option.Match(
            none: AsyncSequence.Return(Option<TItem>.None),
            some: static item => item.Select(Option.Return));

    [Pure]
    public static Task<Option<T>> Traverse<TItem, T>(
        this Option<TItem> option,
        Func<TItem, Task<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    public static Task<Option<TItem>> Sequence<TItem>(
        this Option<Task<TItem>> option)
        where TItem : notnull
        => option.Match(
            none: static () => Task.FromResult(Option<TItem>.None),
            some: static async item => Option.Return(await item.ConfigureAwait(false)));

    [Pure]
    public static ValueTask<Option<T>> Traverse<TItem, T>(
        this Option<TItem> option,
        Func<TItem, ValueTask<T>> selector)
        where TItem : notnull
        where T : notnull
        => option.Select(selector).Sequence();

    [Pure]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1114:Parameter list should follow declaration", Justification = "False positive.")]
    public static ValueTask<Option<TItem>> Sequence<TItem>(
        this Option<ValueTask<TItem>> option)
        where TItem : notnull
        => option.Match<ValueTask<Option<TItem>>>(
            none: static () => ValueTaskFromResult(Option<TItem>.None),
            some: static async item => Option.Return(await item.ConfigureAwait(false)));
}
