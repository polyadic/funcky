#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
    /// <exception cref="ArgumentNullException">Thrown when any value in <paramref name="source"/> is <see langword="null"/>.</exception>
    [Pure]
    public static IAsyncEnumerable<ValueWithPrevious<TSource>> WithPrevious<TSource>(this IAsyncEnumerable<TSource> source)
        where TSource : notnull
        => source.WithPreviousInternal();

    private static async IAsyncEnumerable<ValueWithPrevious<TSource>> WithPreviousInternal<TSource>(this IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TSource : notnull
    {
        var previous = Option<TSource>.None;

        await foreach (var value in source.ConfigureAwait(false).WithCancellation(cancellationToken))
        {
            yield return new ValueWithPrevious<TSource>(value, previous);
            previous = value;
        }
    }
}
#endif
