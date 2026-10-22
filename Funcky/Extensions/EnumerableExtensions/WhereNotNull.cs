namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource?> source)
        where TSource : class
    {
        [Pure]
        public IEnumerable<TSource> WhereNotNull()
            => source.WhereSelect(Option.FromNullable);
    }

    extension<TSource>(IEnumerable<TSource?> source)
        where TSource : struct
    {
        [Pure]
        public IEnumerable<TSource> WhereNotNull()
            => source.WhereSelect(Option.FromNullable);
    }
}
