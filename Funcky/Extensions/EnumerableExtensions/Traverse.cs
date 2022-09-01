using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    [Pure]
    public static Either<TLeft, IReadOnlyList<TRight>> Traverse<TSource, TLeft, TRight>(
        this IEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector)
        where TLeft : notnull
        where TRight : notnull
        => source.Select(selector).Sequence();

    [Pure]
    public static Option<IReadOnlyList<TItem>> Traverse<TSource, TItem>(
        this IEnumerable<TSource> source,
        Func<TSource, Option<TItem>> selector)
        where TItem : notnull
        => source.Select(selector).Sequence();

    [Pure]
    public static Result<IReadOnlyList<TValidResult>> Traverse<TSource, TValidResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Result<TValidResult>> selector)
        => source.Select(selector).Sequence();

    [Pure]
    public static Reader<TEnvironment, IEnumerable<TResult>> Traverse<TSource, TEnvironment, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Reader<TEnvironment, TResult>> selector)
        => source.Select(selector).Sequence();

    public static Lazy<IEnumerable<T>> Traverse<TSource, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(
        IEnumerable<TSource> source,
        Func<TSource, Lazy<T>> selector)
        => source.Select(selector).Sequence();
}
