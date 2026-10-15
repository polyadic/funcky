#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Generates a sequence based on a <paramref name="successor"/> function stopping at the first <see cref="Option{TItem}.None"/> value.
    /// This is essentially the inverse operation of an <see cref="AsyncEnumerable.AggregateAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource, TSource, TSource}, CancellationToken)"/>.
    /// </summary>
    /// <param name="first">The first element of the sequence.</param>
    /// <param name="successor">Generates the next element of the sequence or <see cref="Option{TItem}.None"/> based on the previous item.</param>
    /// <remarks>Use <see cref="AsyncEnumerable.Skip{TSource}(IAsyncEnumerable{TSource}, int)"/> on the result if you don't want the first item to be included.</remarks>
    [Pure]
    public static IAsyncEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, ValueTask<Option<TResult>>> successor)
        where TResult : notnull
        => SuccessorsInternal(first, ArgumentValidator.NotNull(successor));

    /// <inheritdoc cref="Successors{TResult}(Option{TResult}, Func{TResult, ValueTask{Option{TResult}}})" />
    [Pure]
    public static IAsyncEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, ValueTask<Option<TResult>>> successor)
        where TResult : notnull
        => Successors(Option.Some(first), successor);

    /// <inheritdoc cref="Successors{TResult}(Option{TResult}, Func{TResult, ValueTask{Option{TResult}}})" />
    [Pure]
    public static IAsyncEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, ValueTask<TResult>> successor)
        where TResult : notnull
        => Successors(first, ToOptionSuccessor(ArgumentValidator.NotNull(successor)));

    /// <inheritdoc cref="Successors{TResult}(Option{TResult}, Func{TResult, ValueTask{Option{TResult}}})" />
    /// <remarks>Unlike the other overloads, this one never wraps the elements in an <see cref="Option{T}"/>, so <typeparamref name="TResult"/> is not constrained to <c>notnull</c>: it can produce <c>null</c> elements.</remarks>
    [Pure]
    public static IAsyncEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, ValueTask<TResult>> successor)
        => SuccessorsInternal(first, ArgumentValidator.NotNull(successor));

    private static Func<TResult, ValueTask<Option<TResult>>> ToOptionSuccessor<TResult>(Func<TResult, ValueTask<TResult>> successor)
        where TResult : notnull
        => async previous => Option.Some(await successor(previous).ConfigureAwait(false));

    private static async IAsyncEnumerable<TResult> SuccessorsInternal<TResult>(
        Option<TResult> first,
        Func<TResult, ValueTask<Option<TResult>>> successor,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var item = first;
        while (item.TryGetValue(out var itemValue))
        {
            yield return itemValue;
            cancellationToken.ThrowIfCancellationRequested();
            item = await successor(itemValue).ConfigureAwait(false);
        }
    }

    private static async IAsyncEnumerable<TResult> SuccessorsInternal<TResult>(
        TResult first,
        Func<TResult, ValueTask<TResult>> successor,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var item = first;
        while (true)
        {
            yield return item;
            cancellationToken.ThrowIfCancellationRequested();
            item = await successor(item).ConfigureAwait(false);
        }
    }
}
#endif
