namespace Funcky.Extensions;

public static partial class QueryableExtensions
{
#if NET7_0
    private const string InMemoryQueryableExtensionMethodsRequiresDynamicCode = "Enumerating collections as IQueryable can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.";
#endif

    /// <summary>
    /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
    /// </summary>
    /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
    /// <param name="source">The sequence to find an element in.</param>
    /// <param name="index">The index for the element to retrieve.</param>
    /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
    [Pure]
#if NET7_0
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
    public static Option<TSource> ElementAtOrNone<TSource>(this IQueryable<TSource> source, int index)
        where TSource : notnull
        => source
            .Select(x => Option.Some(x))
            .ElementAtOrDefault(index);

#if ELEMENT_AT_INDEX
    /// <summary>
    /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
    /// </summary>
    /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
    /// <param name="source">The sequence to find an element in.</param>
    /// <param name="index">The index for the element to retrieve.</param>
    /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
    [Pure]
#if NET7_0
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
    public static Option<TSource> ElementAtOrNone<TSource>(this IQueryable<TSource> source, Index index)
        where TSource : notnull
        => source
            .Select(x => Option.Some(x))
            .ElementAtOrDefault(index);
#endif
}
