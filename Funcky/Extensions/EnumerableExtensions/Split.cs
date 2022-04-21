using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Splits the source sequence by the given separator.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IEnumerable<TSource> source, TSource separator)
        where TSource : notnull
        => source.Split(separator, EqualityComparer<TSource>.Default);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="equalityComparer">Override the default equality comparer.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(
        this IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> equalityComparer)
        where TSource : notnull
        => source.Split(separator, equalityComparer, Identity);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// With the resultSelector you can chose what to construct from the individual part.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TResult">Type of the elements produced by the <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="equalityComparer">Override the default equality comparer.</param>
    /// <param name="resultSelector">The result selector produces a result from each partial sequence.</param>
    /// <returns>A sequence of results.</returns>
    [Pure]
    public static IEnumerable<TResult> Split<TSource, TResult>(
        this IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> equalityComparer,
        Func<IReadOnlyList<TSource>, TResult> resultSelector)
        where TSource : notnull
    {
        using var sourceEnumerator = source.GetEnumerator();

        while (sourceEnumerator.MoveNext())
        {
            yield return resultSelector(TakeSkipWhile(sourceEnumerator, TakeSkipPredicate(separator, equalityComparer)).ToImmutableList());
        }
    }

    private static Func<TSource, bool> TakeSkipPredicate<TSource>(TSource separator, IEqualityComparer<TSource> equalityComparer)
        where TSource : notnull
        => element
            => !equalityComparer.Equals(element, separator);

    private static IEnumerable<TSource> TakeSkipWhile<TSource>(IEnumerator<TSource> source, Func<TSource, bool> predicate)
    {
        do
        {
            if (predicate(source.Current))
            {
                yield return source.Current;
            }
            else
            {
                yield break;
            }
        }
        while (source.MoveNext());
    }
}
