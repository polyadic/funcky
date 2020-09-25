using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> SlidingWindow<TSource>(this IEnumerable<TSource> source, int width)
        {
            ValidateWindowWidth(width);

            var slidingWindow = new SlidingWindowQueue<TSource>(width);
            foreach (var element in source)
            {
                slidingWindow.Enqueue(element);

                if (slidingWindow.CurrentWidth == width)
                {
                    yield return slidingWindow.Window;
                }
            }
        }

        private static void ValidateWindowWidth(int width)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "The width of the window must be bigger than 0");
            }
        }
    }
}
