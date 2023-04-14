using System.Collections.Immutable;
using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static partial class ImmutableListExtensions
{
    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the list. The value can be <see langword="null" /> for reference types.</param>
    /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    /// <param name="equalityComparer">The equality comparer to match <paramref name="item" />.</param>
    /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> and contains <paramref name="count" /> number of elements if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex, count, equalityComparer));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the list. The value can be <see langword="null" /> for reference types.</param>
    /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item)
        => MapNotFoundToNone(list.LastIndexOf(item));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the list. The value can be <see langword="null" /> for reference types.</param>
    /// <param name="equalityComparer">The equality comparer to match <paramref name="item" />.</param>
    /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, IEqualityComparer<TItem>? equalityComparer)
        => MapNotFoundToNone(list.LastIndexOf(item, equalityComparer));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the list. The value can be <see langword="null" /> for reference types.</param>
    /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex));

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="IImmutableList{T}" /> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements in the <see cref="IImmutableList{T}" /> <paramref name="list"/> sequence.</typeparam>
    /// <param name="list">An <see cref="IImmutableList{T}" /> of values.</param>
    /// <param name="item">The object to locate in the list. The value can be <see langword="null" /> for reference types.</param>
    /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="IImmutableList{T}" /> that starts at <paramref name="startIndex" /> and contains <paramref name="count" /> number of elements if found; otherwise <see cref="Option{T}.None" />.</returns>
    public static Option<int> LastIndexOfOrNone<TItem>(this IImmutableList<TItem> list, TItem item, int startIndex, int count)
        => MapNotFoundToNone(list.LastIndexOf(item, startIndex, count));
}
