using System.Runtime.CompilerServices;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// SlidingWindow returns a sequence of sliding windows of the given width.
        /// The nth sequence will start with the nth element of the source sequence.
        /// </summary>
        /// <remarks>
        /// The returned windows always have  'width' many elements.
        /// i.e. if your source Sequence is smaller than the window, there will be an empty result.
        /// </remarks>
        /// <param name="source">The source sequence.</param>
        /// <param name="width">The width of the sliding window.</param>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <returns>Returns a sequence of equally sized window sequences.</returns>
        [Pure]
        public static IEnumerable<IReadOnlyList<TSource>> SlidingWindow<TSource>(this IEnumerable<TSource> source, int width)
            => SlidingWindowEnumerable(source, ValidateWindowWidth(width));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<IReadOnlyList<TSource>> SlidingWindowEnumerable<TSource>(IEnumerable<TSource> source, int width)
        {
            var slidingWindow = new SlidingWindowQueue<TSource>(width);
            foreach (var element in source)
            {
                if (slidingWindow.Enqueue(element).IsFull)
                {
                    yield return slidingWindow.Window;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ValidateWindowWidth(int width)
            => width > 0
                ? width
                : throw new ArgumentOutOfRangeException(nameof(width), width, "The width of the window must be larger than 0");
    }
}
