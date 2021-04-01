using System;
using System.Collections.Generic;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// The IEnumerable version of foreach. You can apply an action to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }
        }

        /// <summary>
        /// The IEnumerable version of foreach. You can apply an Func{T, Unit} to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static Unit ForEach<T>(this IEnumerable<T> elements, Func<T, Unit> action)
            => elements
                .Aggregate(Unit.Value, (_, element) => action(element));
    }
}
