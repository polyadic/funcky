using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the given sequence eagerly in random Order in O(n).
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <remarks>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method directly or by using foreach.</remarks>
    [Pure]
    public static async ValueTask<IReadOnlyList<TSource>> ShuffleAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        => (await source.ToListAsync(cancellationToken).ConfigureAwait(false)).ToRandomList(new Random());

    [Pure]
    public static async ValueTask<IReadOnlyList<TSource>> ShuffleAsync<TSource>(this IAsyncEnumerable<TSource> source, Random random, CancellationToken cancellationToken = default)
        => (await source.ToListAsync(cancellationToken).ConfigureAwait(false)).ToRandomList(random);
}
