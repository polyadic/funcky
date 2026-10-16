using Funcky.Internal.Validators;

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
        => WhereSelectIterator(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IEnumerable{TSource},Func{TSource,Option{TResult}})"/>
    [Pure]
    public static IEnumerable<TResult> WhereSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, Option<TResult>> selector)
        where TResult : notnull
        => WhereSelectIterator(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));

    private static IEnumerable<TResult> WhereSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
        where TResult : notnull
    {
        foreach (var item in source)
        {
            if (selector(item).TryGetValue(out var result))
            {
                yield return result;
            }
        }
    }

    private static IEnumerable<TResult> WhereSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, Option<TResult>> selector)
        where TResult : notnull
    {
        var index = 0;

        foreach (var item in source)
        {
            if (selector(item, index).TryGetValue(out var result))
            {
                yield return result;
            }

            checked
            {
                index++;
            }
        }
    }
}
