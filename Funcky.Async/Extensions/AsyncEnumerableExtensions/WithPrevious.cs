namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
    /// <exception cref="ArgumentNullException">Thrown when any value in <paramref name="source"/> is <see langword="null"/>.</exception>
    [Pure]
    public static async IAsyncEnumerable<ValueWithPrevious<TSource>> WithPrevious<TSource>(this IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        var previous = Option<TSource>.None;

        await foreach (var value in source)
        {
            yield return new ValueWithPrevious<TSource>(value, previous);
            previous = value;
        }
    }
}
