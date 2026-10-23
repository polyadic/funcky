#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource?> source)
        where TSource : class
    {
        [Pure]
        public IAsyncEnumerable<TSource> WhereNotNull()
            => source.WhereSelect(Option.FromNullable);
    }

    extension<TSource>(IAsyncEnumerable<TSource?> source)
        where TSource : struct
    {
        [Pure]
        public IAsyncEnumerable<TSource> WhereNotNull()
            => source.WhereSelect(Option.FromNullable);
    }
}
#endif
