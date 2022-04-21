namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence. The returned struct is deconstructible.
    /// </summary>
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <returns>Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence.</returns>
    [Pure]
    public static async IAsyncEnumerable<ValueWithLast<TSource>> WithLast<TSource>(this IAsyncEnumerable<TSource> source)
    {
        #pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var enumerator = source.ConfigureAwait(false).GetAsyncEnumerator();
        #pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (!await enumerator.MoveNextAsync())
        {
            yield break;
        }

        var current = enumerator.Current;
        while (await enumerator.MoveNextAsync())
        {
            yield return new ValueWithLast<TSource>(current, false);
            current = enumerator.Current;
        }

        yield return new ValueWithLast<TSource>(current, true);
    }
}
