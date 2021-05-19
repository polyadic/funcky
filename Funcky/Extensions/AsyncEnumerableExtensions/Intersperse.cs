using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public static IAsyncEnumerable<TSource> Intersperse<TSource>(this IAsyncEnumerable<TSource> source, TSource element)
            => AsyncEnumerable.Create(cancellationToken => IntersperseEnumerator(source, element, cancellationToken));

        #pragma warning disable 8425
        private static async IAsyncEnumerator<TSource> IntersperseEnumerator<TSource>(this IAsyncEnumerable<TSource> source, TSource element, CancellationToken cancellationToken)
        {
            var isFirst = true;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (!isFirst)
                {
                    yield return element;
                }

                isFirst = false;

                yield return item;
            }
        }
        #pragma warning restore
    }
}
