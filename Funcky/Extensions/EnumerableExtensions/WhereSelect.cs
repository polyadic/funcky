namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Filters out all the empty values from an <see cref="IEnumerable{T}"><![CDATA[IEnumerable<Option<T>>]]></see> and therefore returns an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> WhereSelect<TSource>(this IEnumerable<Option<TSource>> source)
        where TSource : notnull
        => source.WhereSelect(Identity);

    /// <summary>
    /// Projects and filters an <see cref="IEnumerable{T}"/> at the same time.
    /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> WhereSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
        where TResult : notnull
        => source.SelectMany(input => selector(input).ToEnumerable());

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IEnumerable{TSource},Func{TSource,Option{TResult}})"/>
    [Pure]
    public static IEnumerable<TResult> WhereSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, Option<TResult>> selector)
        where TResult : notnull
        => source.SelectMany((input, index) => selector(input, index).ToEnumerable());
}
