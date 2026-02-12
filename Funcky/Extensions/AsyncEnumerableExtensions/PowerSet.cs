#if INTEGRATED_ASYNC
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// The PowerSet function returns a sequence with the set of all subsets.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>Returns an sequence which includes all subsets of the given sequence.</returns>
    /// <remarks>The PowerSet function returns a sequence with 2^n elements where n is the number of elements int the source sequence.
    /// This means it is only viable for small source sequences.</remarks>
    public static IAsyncEnumerable<IEnumerable<TSource>> PowerSet<TSource>(this IAsyncEnumerable<TSource> source)
        => source.PowerSetInternal();

    private static async IAsyncEnumerable<IEnumerable<TSource>> PowerSetInternal<TSource>(this IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2007 // Configured via IAsyncEnumerable<T> extension
        await using var asyncEnumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
#pragma warning restore CA2007

        await foreach (var set in PowerSetEnumerator(asyncEnumerator).WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return set;
        }
    }

    private static async IAsyncEnumerable<ImmutableStack<TSource>> PowerSetEnumerator<TSource>(this ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator source)
    {
        if (await source.MoveNextAsync())
        {
            var temp = source.Current;
            await foreach (var set in source.PowerSetEnumerator().ConfigureAwait(false))
            {
                yield return set;
                yield return set.Push(temp);
            }
        }
        else
        {
            yield return ImmutableStack<TSource>.Empty;
        }
    }
}
#endif
