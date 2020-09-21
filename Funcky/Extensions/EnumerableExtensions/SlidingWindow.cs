using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> SlidingWindow<TSource>(this IEnumerable<TSource> source, int length)
            where TSource : notnull
        {
            var slidingWindow = ImmutableQueue<TSource>.Empty;
            foreach (var element in source)
            {
                slidingWindow = slidingWindow.Enqueue(element);

                if (slidingWindow.Count() == length + 1)
                {
                    slidingWindow = slidingWindow.Dequeue();
                }

                if (slidingWindow.Count() == length)
                {
                    yield return slidingWindow;
                }
            }
        }
    }
}
