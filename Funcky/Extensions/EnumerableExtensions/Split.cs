using System.Collections.Immutable;
using Funcky.Internal.Validators;

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
        => source.Split(separator, EqualityComparer<TSource>.Default);

    /// <summary>
    /// Splits the source array by the given separator.
    /// </summary>
    /// <remarks>
    /// This overload exists so that arrays keep binding to Funcky's <c>Split</c> in C# 14 and later, where
    /// the first-class span conversion would otherwise make <c>MemoryExtensions.Split(ReadOnlySpan{T}, T)</c> win.
    /// </remarks>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> array.</typeparam>
    /// <param name="source">The source array.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(this TSource[] source, TSource separator)
        => source.Split(separator, EqualityComparer<TSource>.Default);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="comparer">Override the default equality comparer.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(
        this IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> comparer)
        => source.Split(separator, comparer, Identity);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// With the resultSelector you can chose what to construct from the individual part.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TResult">Type of the elements produced by the <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="comparer">Override the default equality comparer.</param>
    /// <param name="resultSelector">The result selector produces a result from each partial sequence.</param>
    /// <returns>A sequence of results.</returns>
    [Pure]
    public static IEnumerable<TResult> Split<TSource, TResult>(
        this IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> comparer,
        Func<IReadOnlyList<TSource>, TResult> resultSelector)
        => SplitIterator(ArgumentValidator.NotNull(source), separator, ArgumentValidator.NotNull(comparer), ArgumentValidator.NotNull(resultSelector));

    private static IEnumerable<TResult> SplitIterator<TSource, TResult>(
        IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> comparer,
        Func<IReadOnlyList<TSource>, TResult> resultSelector)
    {
        var part = ImmutableList.CreateBuilder<TSource>();

        foreach (var element in source)
        {
            if (comparer.Equals(element, separator))
            {
                yield return resultSelector(part.ToImmutable());
                part.Clear();
            }
            else
            {
                part.Add(element);
            }
        }

        yield return resultSelector(part.ToImmutable());
    }
}
