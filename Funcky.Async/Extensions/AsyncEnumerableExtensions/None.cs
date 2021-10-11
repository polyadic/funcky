using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>Determines whether a sequence contains no elements.</summary>
        [Pure]
        public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source)
            => !await source.AnyAsync();

        /// <summary>Determines whether no element exists or satisfies a condition.</summary>
        [Pure]
        public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
            => !await source.AnyAsync(predicate);

        /// <inheritdoc cref="NoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource}, Func{TSource,bool})"/>
        [Pure]
        public static async ValueTask<bool> NoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
            => !await source.AnyAwaitAsync(predicate);

        /// <inheritdoc cref="NoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource}, Func{TSource,bool})"/>
        [Pure]
        public static async ValueTask<bool> NoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
            => !await source.AnyAwaitWithCancellationAsync(predicate);
    }
}
