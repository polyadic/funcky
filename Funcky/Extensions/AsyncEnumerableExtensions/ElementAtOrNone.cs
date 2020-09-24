using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        [Pure]
        public static ValueTask<Option<TSource>> ElementAtOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
            where TSource : notnull
            => source.ElementAtOrNoneWithCancellationAsync(index, cancellationToken);

        [Pure]
        public static async ValueTask<Option<TSource>> ElementAtOrNoneWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            int currentIndex = 0;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (currentIndex == index)
                {
                    return Option.Some(item);
                }
                currentIndex++;
            }

            return Option<TSource>.None();
        }
    }
}
