using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        [Pure]
        public static IAsyncEnumerable<TSource> WhereNotNull<TSource>(this IAsyncEnumerable<TSource?> source)
            where TSource : class
            => source.WhereSelect(Option.FromNullable);

        [Pure]
        public static IAsyncEnumerable<TSource> WhereNotNull<TSource>(this IAsyncEnumerable<TSource?> source)
            where TSource : struct
            => source.WhereSelect(Option.FromNullable);
    }
}
