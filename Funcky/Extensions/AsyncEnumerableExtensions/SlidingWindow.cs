#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;
using Funcky.Internal;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// SlidingWindow returns a sequence of sliding windows of the given width.
    /// The nth sequence will start with the nth element of the source sequence.
    /// </summary>
    /// <remarks>
    /// The returned windows always have <paramref name="width"/> many elements.
    /// i.e. if your source sequence is smaller than the window, there will be an empty result.
    /// </remarks>
    /// <param name="source">The source sequence.</param>
    /// <param name="width">The width of the sliding window.</param>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <returns>Returns a sequence of equally sized window sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<IReadOnlyList<TSource>> SlidingWindow<TSource>(this IAsyncEnumerable<TSource> source, int width)
        => SlidingWindowEnumerable(source, WindowWidthValidator.Validate(width));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async IAsyncEnumerable<IReadOnlyList<TSource>> SlidingWindowEnumerable<TSource>(
        IAsyncEnumerable<TSource> source,
        int width,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var slidingWindow = new SlidingWindowQueue<TSource>(width);
        await foreach (var element in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (slidingWindow.Enqueue(element).IsFull)
            {
                yield return slidingWindow.Window;
            }
        }
    }
}
#endif
