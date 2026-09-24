using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ListExtensions
{
    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Immutable.IImmutableList`1" />. This value can be null for reference types.</param>
    /// <param name="startIndex">The zero-based starting indexes of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    /// <param name="equalityComparer">The equality comparer to use to locate <paramref name="item" />.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> and contains <paramref name="count" /> number of elements if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.IndexOf(item, startIndex, count, equalityComparer));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Immutable.IImmutableList`1" />. This value can be null for reference types.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    [OverloadResolutionPriority(1)]
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item)
        => MapNotFoundToNone(list.IndexOf(item));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Immutable.IImmutableList`1" />. This value can be null for reference types.</param>
    /// <param name="equalityComparer">The equality comparer to use to locate <paramref name="item" />.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.IndexOf(item, equalityComparer));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Immutable.IImmutableList`1" />. This value can be null for reference types.</param>
    /// <param name="startIndex">The zero-based starting indexes of the search. 0 (zero) is valid in an empty list.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex)
        => MapNotFoundToNone(list.IndexOf(item, startIndex));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Immutable.IImmutableList`1" />. This value can be null for reference types.</param>
    /// <param name="startIndex">The zero-based starting indexes of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> and contains <paramref name="count" /> number of elements if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> IndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count)
        => MapNotFoundToNone(list.IndexOf(item, startIndex, count));
}
