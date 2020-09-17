using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> SlidingWindow<TSource>(this IEnumerable<TSource> source, int length)
            where TSource : notnull
        {
            Queue<TSource> slidingWindow = new Queue<TSource>();
            foreach (var element in source)
            {
                slidingWindow.Enqueue(element);

                if (slidingWindow.Count == length + 1)
                {
                    slidingWindow.Dequeue();
                }

                if (slidingWindow.Count == length)
                {
                    yield return slidingWindow;
                }
            }
        }
    }
}
